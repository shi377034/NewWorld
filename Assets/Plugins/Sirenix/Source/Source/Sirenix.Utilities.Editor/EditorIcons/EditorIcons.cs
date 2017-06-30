#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="EditorIcons.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities.Editor
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Collection of EditorIcons for use in GUI drawing.
    /// </summary>
    public static class EditorIcons
    {
        private static EditorIcon airplane;
        private static EditorIcon alertCircle;
        private static EditorIcon alertTriangle;
        private static EditorIcon arrowDown;
        private static EditorIcon arrowLeft;
        private static EditorIcon arrowRight;
        private static EditorIcon arrowUp;
        private static EditorIcon bell;
        private static EditorIcon car;
        private static EditorIcon char1;
        private static EditorIcon char2;
        private static EditorIcon char3;
        private static EditorIcon charGraph;
        private static EditorIcon checkmark;
        private static EditorIcon clock;
        private static EditorIcon clouds;
        private static EditorIcon cloudsRainy;
        private static EditorIcon cloudsRainySunny;
        private static EditorIcon cloudsRainyThunder;
        private static EditorIcon cloudsThunder;
        private static EditorIcon crosshair;
        private static EditorIcon cut;
        private static EditorIcon dayCalendar;
        private static EditorIcon download;
        private static EditorIcon eject;
        private static EditorIcon female;
        private static EditorIcon file;
        private static EditorIcon fileCabinet;
        private static EditorIcon finnishBanner;
        private static EditorIcon flag;
        private static EditorIcon flagFinnish;
        private static EditorIcon folder;
        private static EditorIcon folderBack;
        private static EditorIcon gKey;
        private static EditorIcon globe;
        private static EditorIcon gridBlocks;
        private static EditorIcon gridImageText;
        private static EditorIcon gridImageTextList;
        private static EditorIcon gridLayout;
        private static EditorIcon hamburgerMenu;
        private static EditorIcon house;
        private static EditorIcon image;
        private static EditorIcon imageCollection;
        private static EditorIcon info;
        private static EditorIcon letter;
        private static EditorIcon lightBulb;
        private static EditorIcon link;
        private static EditorIcon list;
        private static EditorIcon loadingBar;
        private static EditorIcon lockLocked;
        private static EditorIcon lockUnloacked;
        private static EditorIcon magnifyingGlass;
        private static EditorIcon male;
        private static EditorIcon marker;
        private static EditorIcon maximize;
        private static EditorIcon microphone;
        private static EditorIcon minimize;
        private static EditorIcon minus;
        private static EditorIcon mobilePhone;
        private static EditorIcon money;
        private static EditorIcon move;
        private static EditorIcon multiUser;
        private static EditorIcon next;
        private static EditorIcon pacmanGhost;
        private static EditorIcon paperclip;
        private static EditorIcon pen;
        private static EditorIcon penAdd;
        private static EditorIcon penDelete;
        private static EditorIcon penMinus;
        private static EditorIcon play;
        private static EditorIcon plus;
        private static EditorIcon podium;
        private static EditorIcon previous;
        private static EditorIcon receptionSignal;
        private static EditorIcon redo;
        private static EditorIcon refresh;
        private static EditorIcon rotate;
        private static EditorIcon ruler;
        private static EditorIcon rulerRect;
        private static EditorIcon settingsCog;
        private static EditorIcon shoppingBasket;
        private static EditorIcon shoppingCart;
        private static EditorIcon singleUser;
        private static EditorIcon smartPhone;
        private static EditorIcon sound;
        private static EditorIcon speechBubbleRound;
        private static EditorIcon speechBubbleSquare;
        private static EditorIcon speechBubblesRound;
        private static EditorIcon speechBubblesSquare;
        private static EditorIcon starPointer;
        private static EditorIcon stop;
        private static EditorIcon stretch;
        private static EditorIcon table;
        private static EditorIcon tag;
        private static EditorIcon testTube;
        private static EditorIcon timer;
        private static EditorIcon trafficStopLight;
        private static EditorIcon transparent;
        private static EditorIcon tree;
        private static EditorIcon triangleDown;
        private static EditorIcon triangleLeft;
        private static EditorIcon triangleRight;
        private static EditorIcon triangleUp;
        private static EditorIcon undo;
        private static EditorIcon upload;
        private static EditorIcon wifiSignal;
        private static EditorIcon x;
        private static Texture2D unityInfoIcon;
        private static Texture2D unityErrorIcon;
        private static Texture2D unityWarningIcon;

        /// <summary>
        /// Gets an icon of an airplane.
        /// </summary>
        public static EditorIcon Airplane { get { return airplane ?? (airplane = new LazyEditorIcon("airplane")); } }

        /// <summary>
        /// Gets an icon of an alertcircle.
        /// </summary>
        public static EditorIcon AlertCircle { get { return alertCircle ?? (alertCircle = new LazyEditorIcon("alertcircle")); } }

        /// <summary>
        /// Gets an icon of an alerttriangle.
        /// </summary>
        public static EditorIcon AlertTriangle { get { return alertTriangle ?? (alertTriangle = new LazyEditorIcon("alerttriangle")); } }

        /// <summary>
        /// Gets an icon of an arrowdown.
        /// </summary>
        public static EditorIcon ArrowDown { get { return arrowDown ?? (arrowDown = new LazyEditorIcon("arrowdown")); } }

        /// <summary>
        /// Gets an icon of an arrowleft.
        /// </summary>
        public static EditorIcon ArrowLeft { get { return arrowLeft ?? (arrowLeft = new LazyEditorIcon("arrowleft")); } }

        /// <summary>
        /// Gets an icon of an arrowright.
        /// </summary>
        public static EditorIcon ArrowRight { get { return arrowRight ?? (arrowRight = new LazyEditorIcon("arrowright")); } }

        /// <summary>
        /// Gets an icon of an arrowup.
        /// </summary>
        public static EditorIcon ArrowUp { get { return arrowUp ?? (arrowUp = new LazyEditorIcon("arrowup")); } }

        /// <summary>
        /// Gets an icon of a bell.
        /// </summary>
        public static EditorIcon Bell { get { return bell ?? (bell = new LazyEditorIcon("bell")); } }

        /// <summary>
        /// Gets an icon of a car.
        /// </summary>
        public static EditorIcon Car { get { return car ?? (car = new LazyEditorIcon("car")); } }

        /// <summary>
        /// Gets an icon of a char1.
        /// </summary>
        public static EditorIcon Char1 { get { return char1 ?? (char1 = new LazyEditorIcon("char1")); } }

        /// <summary>
        /// Gets an icon of a char2.
        /// </summary>
        public static EditorIcon Char2 { get { return char2 ?? (char2 = new LazyEditorIcon("char2")); } }

        /// <summary>
        /// Gets an icon of a char3.
        /// </summary>
        public static EditorIcon Char3 { get { return char3 ?? (char3 = new LazyEditorIcon("char3")); } }

        /// <summary>
        /// Gets an icon of a chargraph.
        /// </summary>
        public static EditorIcon CharGraph { get { return charGraph ?? (charGraph = new LazyEditorIcon("chargraph")); } }

        /// <summary>
        /// Gets an icon of a checkmark.
        /// </summary>
        public static EditorIcon Checkmark { get { return checkmark ?? (checkmark = new LazyEditorIcon("checkmark")); } }

        /// <summary>
        /// Gets an icon of a clock.
        /// </summary>
        public static EditorIcon Clock { get { return clock ?? (clock = new LazyEditorIcon("clock")); } }

        /// <summary>
        /// Gets an icon of a clouds.
        /// </summary>
        public static EditorIcon Clouds { get { return clouds ?? (clouds = new LazyEditorIcon("clouds")); } }

        /// <summary>
        /// Gets an icon of a cloudsrainy.
        /// </summary>
        public static EditorIcon CloudsRainy { get { return cloudsRainy ?? (cloudsRainy = new LazyEditorIcon("cloudsrainy")); } }

        /// <summary>
        /// Gets an icon of a cloudsrainysunny.
        /// </summary>
        public static EditorIcon CloudsRainySunny { get { return cloudsRainySunny ?? (cloudsRainySunny = new LazyEditorIcon("cloudsrainysunny")); } }

        /// <summary>
        /// Gets an icon of a cloudsrainythunder.
        /// </summary>
        public static EditorIcon CloudsRainyThunder { get { return cloudsRainyThunder ?? (cloudsRainyThunder = new LazyEditorIcon("cloudsrainythunder")); } }

        /// <summary>
        /// Gets an icon of a cloudsthunder.
        /// </summary>
        public static EditorIcon CloudsThunder { get { return cloudsThunder ?? (cloudsThunder = new LazyEditorIcon("cloudsthunder")); } }

        /// <summary>
        /// Gets an icon of a crosshair.
        /// </summary>
        public static EditorIcon Crosshair { get { return crosshair ?? (crosshair = new LazyEditorIcon("crosshair")); } }

        /// <summary>
        /// Gets an icon of a cut.
        /// </summary>
        public static EditorIcon Cut { get { return cut ?? (cut = new LazyEditorIcon("cut")); } }

        /// <summary>
        /// Gets an icon of a daycalendar.
        /// </summary>
        public static EditorIcon DayCalendar { get { return dayCalendar ?? (dayCalendar = new LazyEditorIcon("daycalendar")); } }

        /// <summary>
        /// Gets an icon of a download.
        /// </summary>
        public static EditorIcon Download { get { return download ?? (download = new LazyEditorIcon("download")); } }

        /// <summary>
        /// Gets an icon of an eject.
        /// </summary>
        public static EditorIcon Eject { get { return eject ?? (eject = new LazyEditorIcon("eject")); } }

        /// <summary>
        /// Gets an icon of a female.
        /// </summary>
        public static EditorIcon Female { get { return female ?? (female = new LazyEditorIcon("female")); } }

        /// <summary>
        /// Gets an icon of a file.
        /// </summary>
        public static EditorIcon File { get { return file ?? (file = new LazyEditorIcon("file")); } }

        /// <summary>
        /// Gets an icon of a filecabinet.
        /// </summary>
        public static EditorIcon FileCabinet { get { return fileCabinet ?? (fileCabinet = new LazyEditorIcon("filecabinet")); } }

        /// <summary>
        /// Gets an icon of a finnishbanner.
        /// </summary>
        public static EditorIcon FinnishBanner { get { return finnishBanner ?? (finnishBanner = new LazyEditorIcon("finnishbanner")); } }

        /// <summary>
        /// Gets an icon of a flag.
        /// </summary>
        public static EditorIcon Flag { get { return flag ?? (flag = new LazyEditorIcon("flag")); } }

        /// <summary>
        /// Gets an icon of a flagfinnish.
        /// </summary>
        public static EditorIcon FlagFinnish { get { return flagFinnish ?? (flagFinnish = new LazyEditorIcon("flagfinnish")); } }

        /// <summary>
        /// Gets an icon of a folder.
        /// </summary>
        public static EditorIcon Folder { get { return folder ?? (folder = new LazyEditorIcon("folder")); } }

        /// <summary>
        /// Gets an icon of a folderback.
        /// </summary>
        public static EditorIcon FolderBack { get { return folderBack ?? (folderBack = new LazyEditorIcon("folderback")); } }

        /// <summary>
        /// Gets an icon of a gkey.
        /// </summary>
        public static EditorIcon GKey { get { return gKey ?? (gKey = new LazyEditorIcon("gkey")); } }

        /// <summary>
        /// Gets an icon of a globe.
        /// </summary>
        public static EditorIcon Globe { get { return globe ?? (globe = new LazyEditorIcon("globe")); } }

        /// <summary>
        /// Gets an icon of a gridblocks.
        /// </summary>
        public static EditorIcon GridBlocks { get { return gridBlocks ?? (gridBlocks = new LazyEditorIcon("gridblocks")); } }

        /// <summary>
        /// Gets an icon of a gridimagetext.
        /// </summary>
        public static EditorIcon GridImageText { get { return gridImageText ?? (gridImageText = new LazyEditorIcon("gridimagetext")); } }

        /// <summary>
        /// Gets an icon of a gridimagetextlist.
        /// </summary>
        public static EditorIcon GridImageTextList { get { return gridImageTextList ?? (gridImageTextList = new LazyEditorIcon("gridimagetextlist")); } }

        /// <summary>
        /// Gets an icon of a gridlayout.
        /// </summary>
        public static EditorIcon GridLayout { get { return gridLayout ?? (gridLayout = new LazyEditorIcon("gridlayout")); } }

        /// <summary>
        /// Gets an icon of a hamburgermenu.
        /// </summary>
        public static EditorIcon HamburgerMenu { get { return hamburgerMenu ?? (hamburgerMenu = new LazyEditorIcon("hamburgermenu")); } }

        /// <summary>
        /// Gets an icon of a house.
        /// </summary>
        public static EditorIcon House { get { return house ?? (house = new LazyEditorIcon("house")); } }

        /// <summary>
        /// Gets an icon of an image.
        /// </summary>
        public static EditorIcon Image { get { return image ?? (image = new LazyEditorIcon("image")); } }

        /// <summary>
        /// Gets an icon of an image collection.
        /// </summary>
        public static EditorIcon ImageCollection { get { return imageCollection ?? (imageCollection = new LazyEditorIcon("imagecollection")); } }

        /// <summary>
        /// Gets an icon of an info.
        /// </summary>
        public static EditorIcon Info { get { return info ?? (info = new LazyEditorIcon("info")); } }

        /// <summary>
        /// Gets an icon of a letter.
        /// </summary>
        public static EditorIcon Letter { get { return letter ?? (letter = new LazyEditorIcon("letter")); } }

        /// <summary>
        /// Gets an icon of a light bulb.
        /// </summary>
        public static EditorIcon LightBulb { get { return lightBulb ?? (lightBulb = new LazyEditorIcon("lightbulb")); } }

        /// <summary>
        /// Gets an icon of a link.
        /// </summary>
        public static EditorIcon Link { get { return link ?? (link = new LazyEditorIcon("link")); } }

        /// <summary>
        /// Gets an icon of a list.
        /// </summary>
        public static EditorIcon List { get { return list ?? (list = new LazyEditorIcon("list")); } }

        /// <summary>
        /// Gets an icon of a loadingbar.
        /// </summary>
        public static EditorIcon LoadingBar { get { return loadingBar ?? (loadingBar = new LazyEditorIcon("loadingbar")); } }

        /// <summary>
        /// Gets an icon of a locked lock.
        /// </summary>
        public static EditorIcon LockLocked { get { return lockLocked ?? (lockLocked = new LazyEditorIcon("locklocked")); } }

        /// <summary>
        /// Gets an icon of an unloacked lock.
        /// </summary>
        public static EditorIcon LockUnloacked { get { return lockUnloacked ?? (lockUnloacked = new LazyEditorIcon("lockunloacked")); } }

        /// <summary>
        /// Gets an icon of a magnifying glass.
        /// </summary>
        public static EditorIcon MagnifyingGlass { get { return magnifyingGlass ?? (magnifyingGlass = new LazyEditorIcon("magnifyingglass")); } }

        /// <summary>
        /// Gets an icon of a male.
        /// </summary>
        public static EditorIcon Male { get { return male ?? (male = new LazyEditorIcon("male")); } }

        /// <summary>
        /// Gets an icon of a marker.
        /// </summary>
        public static EditorIcon Marker { get { return marker ?? (marker = new LazyEditorIcon("marker")); } }

        /// <summary>
        /// Gets an icon of a maximize symbol.
        /// </summary>
        public static EditorIcon Maximize { get { return maximize ?? (maximize = new LazyEditorIcon("maximize")); } }

        /// <summary>
        /// Gets an icon of a microphone.
        /// </summary>
        public static EditorIcon Microphone { get { return microphone ?? (microphone = new LazyEditorIcon("microphone")); } }

        /// <summary>
        /// Gets an icon of a minimize symbol.
        /// </summary>
        public static EditorIcon Minimize { get { return minimize ?? (minimize = new LazyEditorIcon("minimize")); } }

        /// <summary>
        /// Gets an icon of a minus symbol.
        /// </summary>
        public static EditorIcon Minus { get { return minus ?? (minus = new LazyEditorIcon("minus")); } }

        /// <summary>
        /// Gets an icon of a mobilephone.
        /// </summary>
        public static EditorIcon MobilePhone { get { return mobilePhone ?? (mobilePhone = new LazyEditorIcon("mobilephone")); } }

        /// <summary>
        /// Gets an icon of a money symbol.
        /// </summary>
        public static EditorIcon Money { get { return money ?? (money = new LazyEditorIcon("money")); } }

        /// <summary>
        /// Gets an icon of a move symbol.
        /// </summary>
        public static EditorIcon Move { get { return move ?? (move = new LazyEditorIcon("move")); } }

        /// <summary>
        /// Gets an icon of a multiuser symbol.
        /// </summary>
        public static EditorIcon MultiUser { get { return multiUser ?? (multiUser = new LazyEditorIcon("multiuser")); } }

        /// <summary>
        /// Gets an icon of a next symbol.
        /// </summary>
        public static EditorIcon Next { get { return next ?? (next = new LazyEditorIcon("next")); } }

        /// <summary>
        /// Gets an icon of a pacman ghost.
        /// </summary>
        public static EditorIcon PacmanGhost { get { return pacmanGhost ?? (pacmanGhost = new LazyEditorIcon("pacmanghost")); } }

        /// <summary>
        /// Gets an icon of a paperclip symbol.
        /// </summary>
        public static EditorIcon Paperclip { get { return paperclip ?? (paperclip = new LazyEditorIcon("paperclip")); } }

        /// <summary>
        /// Gets an icon of a pen.
        /// </summary>
        public static EditorIcon Pen { get { return pen ?? (pen = new LazyEditorIcon("pen")); } }

        /// <summary>
        /// Gets an icon of a pen add symbol.
        /// </summary>
        public static EditorIcon PenAdd { get { return penAdd ?? (penAdd = new LazyEditorIcon("penadd")); } }

        /// <summary>
        /// Gets an icon of a pen delete symbol.
        /// </summary>
        public static EditorIcon PenDelete { get { return penDelete ?? (penDelete = new LazyEditorIcon("pendelete")); } }

        /// <summary>
        /// Gets an icon of a pen minus symbol.
        /// </summary>
        public static EditorIcon PenMinus { get { return penMinus ?? (penMinus = new LazyEditorIcon("penminus")); } }

        /// <summary>
        /// Gets an icon of a play symbol.
        /// </summary>
        public static EditorIcon Play { get { return play ?? (play = new LazyEditorIcon("play")); } }

        /// <summary>
        /// Gets an icon of a plus symbol.
        /// </summary>
        public static EditorIcon Plus { get { return plus ?? (plus = new LazyEditorIcon("plus")); } }

        /// <summary>
        /// Gets an icon of a podium.
        /// </summary>
        public static EditorIcon Podium { get { return podium ?? (podium = new LazyEditorIcon("podium")); } }

        /// <summary>
        /// Gets an icon of a previous symbol.
        /// </summary>
        public static EditorIcon Previous { get { return previous ?? (previous = new LazyEditorIcon("previous")); } }

        /// <summary>
        /// Gets an icon of a reception signal.
        /// </summary>
        public static EditorIcon ReceptionSignal { get { return receptionSignal ?? (receptionSignal = new LazyEditorIcon("receptionsignal")); } }

        /// <summary>
        /// Gets an icon of a redo.
        /// </summary>
        public static EditorIcon Redo { get { return redo ?? (redo = new LazyEditorIcon("redo")); } }

        /// <summary>
        /// Gets an icon of a refresh.
        /// </summary>
        public static EditorIcon Refresh { get { return refresh ?? (refresh = new LazyEditorIcon("refresh")); } }

        /// <summary>
        /// Gets an icon of a rotate.
        /// </summary>
        public static EditorIcon Rotate { get { return rotate ?? (rotate = new LazyEditorIcon("rotate")); } }

        /// <summary>
        /// Gets an icon of a ruler.
        /// </summary>
        public static EditorIcon Ruler { get { return ruler ?? (ruler = new LazyEditorIcon("ruler")); } }

        /// <summary>
        /// Gets an icon of a rulerrect.
        /// </summary>
        public static EditorIcon RulerRect { get { return rulerRect ?? (rulerRect = new LazyEditorIcon("rulerrect")); } }

        /// <summary>
        /// Gets an icon of a settings cog.
        /// </summary>
        public static EditorIcon SettingsCog { get { return settingsCog ?? (settingsCog = new LazyEditorIcon("settingscog")); } }

        /// <summary>
        /// Gets an icon of a shopping basket.
        /// </summary>
        public static EditorIcon ShoppingBasket { get { return shoppingBasket ?? (shoppingBasket = new LazyEditorIcon("shoppingbasket")); } }

        /// <summary>
        /// Gets an icon of a shopping cart.
        /// </summary>
        public static EditorIcon ShoppingCart { get { return shoppingCart ?? (shoppingCart = new LazyEditorIcon("shoppingcart")); } }

        /// <summary>
        /// Gets an icon of a single user symbol.
        /// </summary>
        public static EditorIcon SingleUser { get { return singleUser ?? (singleUser = new LazyEditorIcon("singleuser")); } }

        /// <summary>
        /// Gets an icon of a smartphone.
        /// </summary>
        public static EditorIcon SmartPhone { get { return smartPhone ?? (smartPhone = new LazyEditorIcon("smartphone")); } }

        /// <summary>
        /// Gets an icon of a sound.
        /// </summary>
        public static EditorIcon Sound { get { return sound ?? (sound = new LazyEditorIcon("sound")); } }

        /// <summary>
        /// Gets an icon of a round speech bubble.
        /// </summary>
        public static EditorIcon SpeechBubbleRound { get { return speechBubbleRound ?? (speechBubbleRound = new LazyEditorIcon("speechbubbleround")); } }

        /// <summary>
        /// Gets an icon of a quare speech bubble.
        /// </summary>
        public static EditorIcon SpeechBubbleSquare { get { return speechBubbleSquare ?? (speechBubbleSquare = new LazyEditorIcon("speechbubblesquare")); } }

        /// <summary>
        /// Gets an icon of a speechbubblesround.
        /// </summary>
        public static EditorIcon SpeechBubblesRound { get { return speechBubblesRound ?? (speechBubblesRound = new LazyEditorIcon("speechbubblesround")); } }

        /// <summary>
        /// Gets an icon of a speechbubblessquare.
        /// </summary>
        public static EditorIcon SpeechBubblesSquare { get { return speechBubblesSquare ?? (speechBubblesSquare = new LazyEditorIcon("speechbubblessquare")); } }

        /// <summary>
        /// Gets an icon of a starpointer.
        /// </summary>
        public static EditorIcon StarPointer { get { return starPointer ?? (starPointer = new LazyEditorIcon("starpointer")); } }

        /// <summary>
        /// Gets an icon of a stop symbol.
        /// </summary>
        public static EditorIcon Stop { get { return stop ?? (stop = new LazyEditorIcon("stop")); } }

        /// <summary>
        /// Gets an icon of a stretch symbol.
        /// </summary>
        public static EditorIcon Stretch { get { return stretch ?? (stretch = new LazyEditorIcon("stretch")); } }

        /// <summary>
        /// Gets an icon of a table.
        /// </summary>
        public static EditorIcon Table { get { return table ?? (table = new LazyEditorIcon("table")); } }

        /// <summary>
        /// Gets an icon of a tag symbol.
        /// </summary>
        public static EditorIcon Tag { get { return tag ?? (tag = new LazyEditorIcon("tag")); } }

        /// <summary>
        /// Gets an icon of a test tube.
        /// </summary>
        public static EditorIcon TestTube { get { return testTube ?? (testTube = new LazyEditorIcon("testtube")); } }

        /// <summary>
        /// Gets an icon of a timer.
        /// </summary>
        public static EditorIcon Timer { get { return timer ?? (timer = new LazyEditorIcon("timer")); } }

        /// <summary>
        /// Gets an icon of a trafficstoplight.
        /// </summary>
        public static EditorIcon TrafficStopLight { get { return trafficStopLight ?? (trafficStopLight = new LazyEditorIcon("trafficstoplight")); } }

        /// <summary>
        /// Gets a Transparent image.
        /// </summary>
        public static EditorIcon Transparent { get { return transparent ?? (transparent = new LazyEditorIcon("transparent")); } }

        /// <summary>
        /// Gets an icon of a tree.
        /// </summary>
        public static EditorIcon Tree { get { return tree ?? (tree = new LazyEditorIcon("tree")); } }

        /// <summary>
        /// Gets an icon of a triangle down symbol.
        /// </summary>
        public static EditorIcon TriangleDown { get { return triangleDown ?? (triangleDown = new LazyEditorIcon("triangledown")); } }

        /// <summary>
        /// Gets an icon of a triangle left symbol.
        /// </summary>
        public static EditorIcon TriangleLeft { get { return triangleLeft ?? (triangleLeft = new LazyEditorIcon("triangleleft")); } }

        /// <summary>
        /// Gets an icon of a triangle right symbol.
        /// </summary>
        public static EditorIcon TriangleRight { get { return triangleRight ?? (triangleRight = new LazyEditorIcon("triangleright")); } }

        /// <summary>
        /// Gets an icon of a triangle up symbol.
        /// </summary>
        public static EditorIcon TriangleUp { get { return triangleUp ?? (triangleUp = new LazyEditorIcon("triangleup")); } }

        /// <summary>
        /// Gets an icon of a undo symbol.
        /// </summary>
        public static EditorIcon Undo { get { return undo ?? (undo = new LazyEditorIcon("undo")); } }

        /// <summary>
        /// Gets an icon of a upload symbol.
        /// </summary>
        public static EditorIcon Upload { get { return upload ?? (upload = new LazyEditorIcon("upload")); } }

        /// <summary>
        /// Gets an icon of a wifi signal symbol.
        /// </summary>
        public static EditorIcon WifiSignal { get { return wifiSignal ?? (wifiSignal = new LazyEditorIcon("wifisignal")); } }

        /// <summary>
        /// Gets an icon of a x symbol.
        /// </summary>
        public static EditorIcon X { get { return x ?? (x = new LazyEditorIcon("x")); } }

        /// <summary>
        /// Gets an icon of a unity info icon.
        /// </summary>
        public static Texture2D UnityInfoIcon { get { return unityInfoIcon ?? (unityInfoIcon = (Texture2D)typeof(EditorGUIUtility).GetMethod("LoadIcon", Flags.AllMembers).Invoke(null, new object[] { "console.infoicon" })); } }

        /// <summary>
        /// Gets an icon of a unity warning icon.
        /// </summary>
        public static Texture2D UnityWarningIcon { get { return unityWarningIcon ?? (unityWarningIcon = (Texture2D)typeof(EditorGUIUtility).GetMethod("LoadIcon", Flags.AllMembers).Invoke(null, new object[] { "console.warnicon" })); } }

        /// <summary>
        /// Gets an icon of a unity error icon.
        /// </summary>
        public static Texture2D UnityErrorIcon { get { return unityErrorIcon ?? (unityErrorIcon = (Texture2D)typeof(EditorGUIUtility).GetMethod("LoadIcon", Flags.AllMembers).Invoke(null, new object[] { "console.erroricon" })); } }
    }
}
#endif