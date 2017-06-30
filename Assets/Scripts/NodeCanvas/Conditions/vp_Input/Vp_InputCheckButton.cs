using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{
    [Name("CheckButton")]
	[Category("Vp_Input")]
	public class Vp_InputCheckButton : ConditionTask{

        public PressTypes pressType = PressTypes.Down;

        [RequiredField]
        public BBParameter<string> buttonName = "Fire1";

        protected override string info
        {
            get { return pressType.ToString() + " " + buttonName.ToString(); }
        }

        protected override bool OnCheck()
        {

            if (pressType == PressTypes.Down)
                return vp_Input.GetButtonDown(buttonName.value);

            if (pressType == PressTypes.Up)
                return vp_Input.GetButtonUp(buttonName.value);

            if (pressType == PressTypes.Pressed)
                return vp_Input.GetButton(buttonName.value);

            return false;
        }
    }
}