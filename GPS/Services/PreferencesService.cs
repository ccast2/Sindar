using System;
using Android.Content;
using Android.Preferences;

namespace GPS.Services
{
    class PreferencesService
    {
        private ISharedPreferences mSharedPrefs;
        private ISharedPreferencesEditor mPrefsEditor;
        private Context mContext;

        private static String PREFERENCE_ACCESS_KEY = "PREFERENCE_ACCESS_KEY";
        public PreferencesService(Context context)
        {
            this.mContext = context;
            mSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            mPrefsEditor = mSharedPrefs.Edit();
        }

        public void saveValue(string key)
        {
            mPrefsEditor.PutString(PREFERENCE_ACCESS_KEY, key);
            mPrefsEditor.Commit();
        }
        public string getAccessKey()
        {
            return mSharedPrefs.GetString(PREFERENCE_ACCESS_KEY, "");
        }
    }
}