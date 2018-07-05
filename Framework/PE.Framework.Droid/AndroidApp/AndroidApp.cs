namespace PE.Framework.Droid.AndroidApp.AppVersion
{
	public class AndroidApp : IAndroidApp
	{
		private object _activity;
		public object TopActivity
        {
            get
            {
				return _activity;
            }
            
            set
            {
				_activity = value;
            }
        }

	}
}
