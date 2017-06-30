﻿#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Editor;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Framework{

	partial class Graph {

		private Rect blackboardRect = new Rect(15, 55, 0, 0);
		private Rect inspectorRect  = new Rect(15, 55, 0, 0);
		private Vector2 nodeInspectorScrollPos;
		private Vector2 blackboardInspectorScrollPos;
		private Graph _currentChildGraph;

		private static object _currentSelection;
		private static List<object> _multiSelection = new List<object>();

		public static System.Action PostGUI{get;set;}
		public static bool allowClick{get;set;}

		virtual protected bool canAcceptVariableDrops{get{return false;}}

		//responsible for the breacrumb navigation
		public Graph currentChildGraph{
			get {return _currentChildGraph;}
			set
			{
				if (Application.isPlaying && value != null && EditorUtility.IsPersistent(value)){
					Debug.LogWarning("You can't view sub-graphs in play mode until they are initialized, to avoid editing asset references accidentally");
					return;
				}

				Undo.RecordObject(this, "Change View");
				if (value != null){
					value.currentChildGraph = null;
				}
				_currentChildGraph = value;
			}
		}

		//Selected Node or Connection
		public static object currentSelection{
			get
			{
				if (multiSelection.Count > 1){
					return null;
				}
				if (multiSelection.Count == 1){
					return multiSelection[0];
				}
				return _currentSelection;
			}
			set
			{
				if (!multiSelection.Contains(value)){
					multiSelection.Clear();
				}
				_currentSelection = value;
				GUIUtility.keyboardControl = 0;
				SceneView.RepaintAll(); //for gizmos
			}
		}

		public static List<object> multiSelection{
			get {return _multiSelection;}
			set
			{
				if (value != null && value.Count == 1){
					currentSelection = value[0];
					value.Clear();
				}
				_multiSelection = value != null? value : new List<object>();
			}
		}

		private static Node selectedNode{
			get {return currentSelection as Node;}
		}

		private static Connection selectedConnection{
			get	{return currentSelection as Connection;}
		}

        private string exportFileExtension{
            get { return this.GetType().Name.GetCapitals(); }
        }

        private float screenWidth{ //for retina
        	get {return EditorGUIUtility.currentViewWidth;}
        }

        private float screenHeight{
        	get {return Screen.height;}
        }

		///

		///Clears the whole graph
		public void ClearGraph(){
			canvasGroups = null;
			foreach (var node in allNodes.ToArray()){
				RemoveNode(node);
			}
		}

		//This is called while within Begin/End windows from the GraphEditor.
		public void ShowNodesGUI(Event e, Rect drawCanvas, bool fullDrawPass, Vector2 canvasMousePos, float zoomFactor){

			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;

			for (var i = 0; i < allNodes.Count; i++){

				//ensure IDs are updated
				if (allNodes[i].ID != i + 1){
					UpdateNodeIDs(true);
					break;
				}

				allNodes[i].ShowNodeGUI(drawCanvas, fullDrawPass, canvasMousePos, zoomFactor);
			}

			if (primeNode != null){
				GUI.Box(new Rect(primeNode.nodeRect.x, primeNode.nodeRect.y - 20, primeNode.nodeRect.width, 20), "<b>START</b>");
			}
		}

		//This is called outside of windows
		public void ShowGraphControls(Event e, Vector2 canvasMousePos){

			ShowToolbar(e);
			ShowInspectorGUIPanel(e, canvasMousePos);
			ShowBlackboardGUIPanel(e, canvasMousePos);
			ShowGraphCommentsGUI(e, canvasMousePos);
			HandleEvents(e, canvasMousePos);
			AcceptDrops(e, canvasMousePos);

			if (PostGUI != null){
				PostGUI();
				PostGUI = null;
			}
		}

		//This is called outside Begin/End Windows from GraphEditor.
		void ShowToolbar(Event e){

			var owner = this.agent != null && agent is GraphOwner && (agent as GraphOwner).graph == this? (GraphOwner)agent : null;


			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUI.backgroundColor = new Color(1f,1f,1f,0.5f);

			///FILE
			if (GUILayout.Button("File", EditorStyles.toolbarDropDown, GUILayout.Width(50))){
				var menu = new GenericMenu();

#if !UNITY_WEBPLAYER

				//Import JSON
				menu.AddItem (new GUIContent ("Import JSON"), false, ()=>
				{
	                if (allNodes.Count > 0 && !EditorUtility.DisplayDialog("Import Graph", "All current graph information will be lost. Are you sure?", "YES", "NO"))
	                    return;

	                var path = EditorUtility.OpenFilePanel( string.Format("Import '{0}' Graph", this.GetType().Name), "Assets", exportFileExtension);
	                if (!string.IsNullOrEmpty(path)){
	                    if ( this.Deserialize( System.IO.File.ReadAllText(path), true, null ) == null){ //true: validate, null: this._objectReferences
	                        EditorUtility.DisplayDialog("Import Failure", "Please read the logs for more information", "OK", "");
	                    }
	                }
				});

				//Expot JSON
				menu.AddItem (new GUIContent ("Export JSON"), false, ()=>
				{
					var path = EditorUtility.SaveFilePanelInProject (string.Format("Export '{0}' Graph", this.GetType().Name), "", exportFileExtension, "");
	                if (!string.IsNullOrEmpty(path)){
	                    System.IO.File.WriteAllText( path, this.Serialize(true, null) ); //true: pretyJson, null: this._objectReferences
	                    AssetDatabase.Refresh();
	                }
				});
#else
	
				menu.AddDisabledItem(new GUIContent("Import JSON (not possible with webplayer active platform)"));
				menu.AddDisabledItem(new GUIContent("Export JSON (not possible with webplayer active platform)"));

#endif
				menu.ShowAsContext();
			}

			///EDIT
			if (GUILayout.Button("Edit", EditorStyles.toolbarDropDown, GUILayout.Width(50))){
				var menu = new GenericMenu();

				//Bind
				if (!Application.isPlaying && owner != null && !owner.graphIsBound){
					menu.AddItem(new GUIContent("Bind To Owner"), false, ()=>
					{
						if (EditorUtility.DisplayDialog("Bind Graph", "This will make a local copy of the graph, bound to the owner.\n\nThis allows you to make local changes and assign scene object references directly.\n\nNote that you can also use scene object references through the use of Blackboard Variables.\n\nBind Graph?", "YES", "NO")){
							Undo.RecordObject(owner, "New Local Graph");
							owner.SetBoundGraphReference(owner.graph);
							EditorUtility.SetDirty(owner);
						}
					});
				}
				else menu.AddDisabledItem(new GUIContent("Bind To Owner"));

				//Save to asset
				if (owner != null && owner.graphIsBound){
					menu.AddItem(new GUIContent("Save To Asset"), false, ()=>
					{
						var newGraph = (Graph)EditorUtils.CreateAsset(this.GetType(), true);
						if (newGraph != null){
							EditorUtility.CopySerialized(this, newGraph);
							newGraph.Validate();
							AssetDatabase.SaveAssets();
						}
					});
				}
				else menu.AddDisabledItem(new GUIContent("Save To Asset"));

				//Create defined vars
				if (blackboard != null){
					menu.AddItem(new GUIContent("Promote Defined Parameters To Variables"), false, ()=>
					{
						if (EditorUtility.DisplayDialog("Promote Defined Parameters", "This will fill the current Blackboard with a Variable for each defined Parameter in the graph.\nContinue?", "YES", "NO")){
							CreateDefinedParameterVariables(blackboard);
						}
					});
				}
				else menu.AddDisabledItem(new GUIContent("Promote Defined Parameters To Variables"));

				menu.ShowAsContext();
			}
			//////

				
			///PREFS
			if (GUILayout.Button("Prefs", EditorStyles.toolbarDropDown, GUILayout.Width(50))){
				var menu = new GenericMenu();
				menu.AddItem (new GUIContent ("Use Node Browser"), NCPrefs.useBrowser, ()=> {NCPrefs.useBrowser = !NCPrefs.useBrowser;});
				menu.AddItem (new GUIContent ("Show Icons"), NCPrefs.showIcons, ()=>
					{
						NCPrefs.showIcons = !NCPrefs.showIcons;
						foreach(var node in allNodes){ node.nodeRect = new Rect( node.nodePosition.x, node.nodePosition.y, Node.minSize.x, Node.minSize.y ); }
					});
				menu.AddItem (new GUIContent ("Show Node Help"), NCPrefs.showNodeInfo, ()=> {NCPrefs.showNodeInfo = !NCPrefs.showNodeInfo;});
				menu.AddItem (new GUIContent ("Show Comments"), NCPrefs.showComments, ()=> {NCPrefs.showComments = !NCPrefs.showComments;});
				menu.AddItem (new GUIContent ("Show Summary Info"), NCPrefs.showTaskSummary, ()=> {NCPrefs.showTaskSummary = !NCPrefs.showTaskSummary;});
				menu.AddItem (new GUIContent ("Show Node IDs"), NCPrefs.showNodeIDs, ()=> {NCPrefs.showNodeIDs = !NCPrefs.showNodeIDs;});
				menu.AddItem (new GUIContent ("Log Events"), NCPrefs.logEvents, ()=>{ NCPrefs.logEvents = !NCPrefs.logEvents; });
				menu.AddItem (new GUIContent ("Grid Snap"), NCPrefs.doSnap, ()=> {NCPrefs.doSnap = !NCPrefs.doSnap;});
				menu.AddItem (new GUIContent ("Breakpoints Pause Editor"), NCPrefs.breakpointPauseEditor, ()=> {NCPrefs.breakpointPauseEditor = !NCPrefs.breakpointPauseEditor;});
				if (autoSort){
					menu.AddItem (new GUIContent ("Automatic Hierarchical Move"), NCPrefs.hierarchicalMove, ()=> {NCPrefs.hierarchicalMove = !NCPrefs.hierarchicalMove;});
				}
				menu.AddItem (new GUIContent ("Connection Style/Smooth"), NCPrefs.connectionStyle == NCPrefs.ConnectionStyle.Smooth, ()=> {NCPrefs.connectionStyle = NCPrefs.ConnectionStyle.Smooth;});
				menu.AddItem (new GUIContent ("Connection Style/Stepped"), NCPrefs.connectionStyle == NCPrefs.ConnectionStyle.Stepped, ()=> {NCPrefs.connectionStyle = NCPrefs.ConnectionStyle.Stepped;});
				menu.AddItem (new GUIContent ("Connection Style/Straight"), NCPrefs.connectionStyle == NCPrefs.ConnectionStyle.Straight, ()=> {NCPrefs.connectionStyle = NCPrefs.ConnectionStyle.Straight;});
				menu.AddItem( new GUIContent ("Open Preferred Types Editor..."), false, ()=>{PreferedTypesEditorWindow.ShowWindow();} );
				menu.ShowAsContext();
			}
			/////////

			GUILayout.Space(10);

			////CLICK SELECT
			if (owner != null && GUILayout.Button("Select Owner", EditorStyles.toolbarButton, GUILayout.Width(80))){
				Selection.activeObject = owner;
				EditorGUIUtility.PingObject(owner);
			}

			if (EditorUtility.IsPersistent(this) && GUILayout.Button("Select Graph", EditorStyles.toolbarButton, GUILayout.Width(80))){
				Selection.activeObject = this;
				EditorGUIUtility.PingObject(this);
			}
			////////////////

			GUILayout.Space(10);
			GUILayout.FlexibleSpace();

			//DROPDOWN GRAPHOWNER JUMP SELECTION
			if (owner != null && !NCPrefs.isLocked){
				if (GUILayout.Button(string.Format("[{0}]", owner.gameObject.name), EditorStyles.toolbarDropDown, GUILayout.Width(120))){
					var menu = new GenericMenu();
					foreach(var _o in FindObjectsOfType<GraphOwner>()){
						var o = _o;
						menu.AddItem (new GUIContent(o.GetType().Name + "s/" + o.gameObject.name), false, ()=> { Selection.activeObject = o; Selection.selectionChanged(); });
					}
					menu.ShowAsContext();
				}
			}
			////////////////////////////////////

			NCPrefs.isLocked = GUILayout.Toggle(NCPrefs.isLocked, "Lock", EditorStyles.toolbarButton);
			GUILayout.Space(2);

			GUI.backgroundColor = new Color(1, 0.8f, 0.8f, 1);
			if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50))){
				if (EditorUtility.DisplayDialog("Clear Canvas", "This will delete all nodes of the currently viewing graph!\nAre you sure?", "YES", "NO!")){
					ClearGraph();
					e.Use();
					return;
				}
			}

			GUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;
		}

		void HandleEvents(Event e, Vector2 canvasMousePos){

			//we also undo graph pans
			if (e.type == EventType.MouseDown && e.button == 2){
	    		Undo.RegisterCompleteObjectUndo(this, "Graph Pan");
	    	}

			//variable is set as well, so that  nodes know if they can be clicked
			var inspectorWithScrollbar = new Rect(inspectorRect.x, inspectorRect.y, inspectorRect.width + 14, inspectorRect.height);
			allowClick = !inspectorWithScrollbar.Contains(e.mousePosition) && !blackboardRect.Contains(e.mousePosition);
			if (!allowClick){
				return;
			}

			//Shortcuts
			if (e.type == EventType.KeyUp && GUIUtility.keyboardControl == 0){
				
				//Delete
				if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace){

					if (multiSelection != null && multiSelection.Count > 0){
						foreach (var obj in multiSelection.ToArray()){
							if (obj is Node){
								RemoveNode(obj as Node);
							}
							if (obj is Connection){
								RemoveConnection(obj as Connection);
							}
						}
						multiSelection = null;
					}

					if (selectedNode != null){
						RemoveNode(selectedNode);
						currentSelection = null;
					}

					if (selectedConnection != null){
						RemoveConnection(selectedConnection);
						currentSelection = null;
					}
					e.Use();
				}

				//Duplicate
				if (e.keyCode == KeyCode.D && e.control){
					if (multiSelection != null && multiSelection.Count > 0){
						var newNodes = CopyNodesToGraph(multiSelection.OfType<Node>().ToList(), this);
						multiSelection = newNodes.Cast<object>().ToList();
					}
					if (selectedNode != null){
						currentSelection = selectedNode.Duplicate(this);
					}
					//Connections can't be duplicated by themselves. They do so as part of multiple node duplication though.
					e.Use();
				}
			}


			//Tilt '`' or 'space' opens up the compelte context menu browser
			if (e.type == EventType.KeyDown && !e.shift && (e.keyCode == KeyCode.BackQuote || e.keyCode == KeyCode.Space) ){
				CompleteContextMenu.Show( GetAddNodeMenu(canvasMousePos), e.mousePosition, string.Format("Add {0} Node", this.GetType().FriendlyName()), baseNodeType );
			}


			//Right click canvas context menu. Basicaly for adding new nodes.
			if (e.type == EventType.ContextClick){
				
				var menu = GetAddNodeMenu(canvasMousePos);

				if (Node.copiedNodes != null && Node.copiedNodes[0].GetType().IsSubclassOf(baseNodeType)){
					menu.AddSeparator("/");
					if (Node.copiedNodes.Length == 1){
						menu.AddItem(new GUIContent(string.Format("Paste Node ({0})", Node.copiedNodes[0].GetType().Name )), false, ()=>
							{
								var newNode = Node.copiedNodes[0].Duplicate(this);
								newNode.nodePosition = canvasMousePos;
								currentSelection = newNode;
							});

					} else if (Node.copiedNodes.Length > 1){
						menu.AddItem(new GUIContent(string.Format("Paste Nodes ({0})", Node.copiedNodes.Length.ToString() )), false, ()=>
						{
							var newNodes = Graph.CopyNodesToGraph(Node.copiedNodes.ToList(), this);
							var diff = newNodes[0].nodeRect.center - canvasMousePos;
							newNodes[0].nodePosition = canvasMousePos;
							for (var i = 1; i < newNodes.Count; i++){
								newNodes[i].nodePosition -= diff;
							}
							multiSelection = newNodes.Cast<object>().ToList();
						});
					}
				}

				if (NCPrefs.useBrowser){
					CompleteContextMenu.Show( menu, e.mousePosition, string.Format("Add {0} Node", this.GetType().FriendlyName()), baseNodeType );
				} else {
					menu.ShowAsContext();
				}
				e.Use();
			}
		}
		
		///The final generic menu used for adding nodes in the canvas
		GenericMenu GetAddNodeMenu(Vector2 canvasMousePos){
			System.Action<System.Type> Selected = (type) =>	{ currentSelection = AddNode(type, canvasMousePos); };
			var menu = EditorUtils.GetTypeSelectionMenu(baseNodeType, Selected);
			menu = OnCanvasContextMenu(menu, canvasMousePos);
			return menu;
		}


		///Override to add extra context sensitive options in the right click canvas context menu
		virtual protected GenericMenu OnCanvasContextMenu(GenericMenu menu, Vector2 canvasMousePos){
			return menu;
		}

		//Show the comments window
		void ShowGraphCommentsGUI(Event e, Vector2 canvasMousePos){

			if (NCPrefs.showComments && !string.IsNullOrEmpty(graphComments)){
				GUI.backgroundColor = new Color(1f,1f,1f,0.3f);
				GUI.Box(new Rect(10, screenHeight - 100, 330, 60), graphComments, (GUIStyle)"textArea");
				GUI.backgroundColor = Color.white;
			}
		}

		//This is the window shown at the top left with a GUI for extra editing opions of the selected node.
		void ShowInspectorGUIPanel(Event e, Vector2 canvasMousePos){

			if ( (selectedNode == null && selectedConnection == null) || NCPrefs.useExternalInspector){
				inspectorRect.height = 0;
				return;
			}

			inspectorRect.width = 330;
			inspectorRect.x = 10;
			inspectorRect.y = 30;

			EditorGUIUtility.AddCursorRect(new Rect(inspectorRect.x, inspectorRect.y, 330, 30), MouseCursor.Link);
			if (GUI.Button(new Rect( inspectorRect.x, inspectorRect.y, 330, 30 ), "")){
				NCPrefs.showNodePanel = !NCPrefs.showNodePanel;
			}

			GUI.Box(inspectorRect, "", (GUIStyle)"windowShadow");
			var title = selectedNode != null? selectedNode.name : "Connection";
			if (NCPrefs.showNodePanel){

				var lastSkin = GUI.skin;
				var viewRect = new Rect(inspectorRect.x, inspectorRect.y, inspectorRect.width + 18, screenHeight - inspectorRect.y - 30);
				nodeInspectorScrollPos = GUI.BeginScrollView(viewRect, nodeInspectorScrollPos, inspectorRect);

				GUILayout.BeginArea(inspectorRect, title, (GUIStyle)"editorPanel");
				GUILayout.Space(5);
				GUI.skin = null;

				if (selectedNode != null){
					selectedNode.ShowNodeInspectorGUI();
				} else if (selectedConnection != null){
					selectedConnection.ShowConnectionInspectorGUI();
				}

				GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(inspectorRect.width - 10));
				GUI.skin = lastSkin;
				if (e.type == EventType.Repaint){
					inspectorRect.height = GUILayoutUtility.GetLastRect().yMax + 5;
				}

				GUILayout.EndArea();
				GUI.EndScrollView();

				if (GUI.changed){
					EditorUtility.SetDirty(this);
				}

			} else {

				GUI.Box(inspectorRect, title, (GUIStyle)"editorPanel");
				inspectorRect.height = 55;
				GUI.color = new Color(1,1,1,0.2f);
				var r = new Rect(inspectorRect.x,inspectorRect.y + 30, inspectorRect.width, 20);
				EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
				if (GUI.Button(r, "...")){
					NCPrefs.showNodePanel = true;
				}
				GUI.color = Color.white;
			}
		}


		//Show the target blackboard window
		void ShowBlackboardGUIPanel(Event e, Vector2 canvasMousePos){

			if (blackboard == null){
				blackboardRect.height = 0;
				return;
			}

			blackboardRect.x = screenWidth - 350;

			blackboardRect.y = 30;
			blackboardRect.width = 330;
			
			EditorGUIUtility.AddCursorRect(new Rect(blackboardRect.x, blackboardRect.y, 330, 30), MouseCursor.Link);
			if (GUI.Button(new Rect( blackboardRect.x, blackboardRect.y, 330, 30 ), "")){
				NCPrefs.showBlackboard = !NCPrefs.showBlackboard;
			}

			GUI.Box(blackboardRect, "", (GUIStyle)"windowShadow");
			var title = blackboard == localBlackboard? string.Format("Local {0} Variables", this.GetType().Name) : "Variables";
			if (NCPrefs.showBlackboard){

				var lastSkin = GUI.skin;
				var viewRect = new Rect(blackboardRect.x, blackboardRect.y, blackboardRect.width + 16, screenHeight - blackboardRect.y - 30);
				var r = new Rect(blackboardRect.x - 3, blackboardRect.y, blackboardRect.width, blackboardRect.height);
				blackboardInspectorScrollPos = GUI.BeginScrollView(viewRect, blackboardInspectorScrollPos, r);

				GUILayout.BeginArea(blackboardRect, title, (GUIStyle)"editorPanel");
				GUILayout.Space(5);
				GUI.skin = null;

				BlackboardEditor.ShowVariables(blackboard, blackboard == localBlackboard? this : blackboard as Object );
				GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(blackboardRect.width - 10));
				GUI.skin = lastSkin;
				if (e.type == EventType.Repaint){
					blackboardRect.height = GUILayoutUtility.GetLastRect().yMax + 5;
				}
				GUILayout.EndArea();
				GUI.EndScrollView();

			} else {
				GUI.Box(blackboardRect, title, (GUIStyle)"editorPanel");
				blackboardRect.height = 55;
				var r = new Rect(blackboardRect.x,blackboardRect.y + 30, blackboardRect.width, 20);
				GUI.color = new Color(1,1,1,0.2f);
				EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
				if (GUI.Button(r, "...")){
					NCPrefs.showBlackboard = true;
				}
				GUI.color = Color.white;
			}

			
			if (canAcceptVariableDrops && BlackboardEditor.pickedVariable != null && BlackboardEditor.pickedVariableBlackboard == blackboard){
				GUI.Label(new Rect(e.mousePosition.x + 15, e.mousePosition.y, 100, 18), "Drop Variable");
				if (e.type == EventType.MouseUp && !blackboardRect.Contains(e.mousePosition)){
					OnVariableDropInGraph(BlackboardEditor.pickedVariable, e, canvasMousePos);
					BlackboardEditor.pickedVariable = null;
					BlackboardEditor.pickedVariableBlackboard = null;
				}
			}
		}

 
 		//Handles Drag&Drop operations
		void AcceptDrops(Event e, Vector2 canvasMousePos){

			if (allowClick){

				if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length == 1){

					if (e.type == EventType.DragUpdated){
						DragAndDrop.visualMode = DragAndDropVisualMode.Link;
					}

					if (e.type == EventType.DragPerform){
						var value = DragAndDrop.objectReferences[0];
						DragAndDrop.AcceptDrag();
						OnDropAccepted(value, canvasMousePos);
					}
				}
			}
		}

		///Handles drag and drop objects in the graph
		virtual protected void OnDropAccepted(Object o, Vector2 canvasMousePos){}

		///Handle what happens when blackboard variable is drag&droped in graph
		virtual protected void OnVariableDropInGraph(Variable variable, Event e, Vector2 canvasMousePos){}

		[ContextMenu("Deep Duplicate")]
		public void DeepDuplicate(){
			if (EditorUtility.DisplayDialog("Deep Duplicate", "This will create a deep duplicate of this graph asset along with it's subgraphs. Continue?", "Yes", "No")){
				DeepCopy(this);
			}
		}

		///Make a deep copy of provided graph asset along with it's sub-graphs.
		static Graph DeepCopy(Graph root){
			if (root == null){
				return null;
			}

			var path = EditorUtility.SaveFilePanelInProject ("Duplicate of " + root.name, root.name + "_duplicate.asset", "asset", "");
			if (string.IsNullOrEmpty(path)){
				return null;
			}

			var copy = (Graph)ScriptableObject.CreateInstance(root.GetType());
			AssetDatabase.CreateAsset(copy, path);
			EditorUtility.CopySerialized(root, copy);

			//make use of IGraphAssignable interface to find nodes that represent a sub-graph.
			foreach(var subGraphNode in copy.allNodes.OfType<IGraphAssignable>()){
				if (subGraphNode.nestedGraph != null){
					//duplicate the existing sub-graph and assign the copy to node.
					subGraphNode.nestedGraph = DeepCopy(subGraphNode.nestedGraph);;
				}
			}

			copy.Validate();
			AssetDatabase.SaveAssets();
			return copy;			
		}
	}
}

#endif
