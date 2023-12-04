namespace Services
{
	public class ParamServices
	{
		public PopupType PopupTypeParam { get; set; }
		public bool IsContinue { get; set; }
	}

	public enum PopupType
	{
		SettingPopup,
		GameOverPopup
	}
}
