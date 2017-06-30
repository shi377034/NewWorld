using UnityEngine;

public class vComment : MonoBehaviour
{
	#if UNITY_EDITOR
	[TextAreaAttribute (5, 3000)]
	public string comment;
	#endif
}
