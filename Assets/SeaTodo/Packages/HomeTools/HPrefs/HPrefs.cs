namespace HomeTools.HPrefs
{
    // A class that has analogs of PlayerPrefs fields for saving to a file
    public static class HPrefs
    {
        #region general

        public static bool HasKey(string key)
        {
            return HPrefsSerialization.IntData.ContainsKey(key) || 
                   HPrefsSerialization.StringData.ContainsKey(key) || 
                   HPrefsSerialization.FloatData.ContainsKey(key);
        }

        public static void DeleteKey(string key)
        {
            var valueFloat = GetFloat(key);
            var valueInt = GetInt(key);
            var valueString = GetString(key);

            if (HPrefsSerialization.FloatData.ContainsKey(key))
            {
                HPrefsSerialization.FloatData.Remove(key);
            }

            if (HPrefsSerialization.IntData.ContainsKey(key))
            {
                HPrefsSerialization.IntData.Remove(key);
            }

            if (HPrefsSerialization.StringData.ContainsKey(key))
            {
                HPrefsSerialization.StringData.Remove(key);
            }
        }

        public static void DeleteAll()
        {
            HPrefsSerialization.FloatData.Clear();
            HPrefsSerialization.IntData.Clear();
            HPrefsSerialization.StringData.Clear();
            HPrefsSerialization.SaveDataToFile();
        }

        #endregion general

        #region int

        public static int GetInt(string key)
        {
            if (HPrefsSerialization.IntData.ContainsKey(key))
            {
                return HPrefsSerialization.IntData[key];
            }
            else
            {
                return 0;
            }
        }

        public static void SetInt(string key, int value)
        {
            if (HPrefsSerialization.IntData.ContainsKey(key))
            {
                HPrefsSerialization.IntData[key] = value;
            }
            else
            {
                HPrefsSerialization.IntData.Add(key, value);
            }
        }

        #endregion int

        #region float

        public static float GetFloat(string key)
        {
            if (HPrefsSerialization.FloatData.ContainsKey(key))
            {
                return HPrefsSerialization.FloatData[key];
            }
            else
            {
                return 0f;
            }
        }

        public static void SetFloat(string key, float value)
        {
            if (HPrefsSerialization.FloatData.ContainsKey(key))
            {
                HPrefsSerialization.FloatData[key] = value;
            }
            else
            {
                HPrefsSerialization.FloatData.Add(key, value);
            }
        }

        #endregion float

        #region string

        public static string GetString(string key)
        {
            if (HPrefsSerialization.StringData.ContainsKey(key))
            {
                return HPrefsSerialization.StringData[key];
            }
            else
            {
                return string.Empty;
            }
        }

        public static void SetString(string key, string value)
        {
            if (HPrefsSerialization.StringData.ContainsKey(key))
            {
                HPrefsSerialization.StringData[key] = value;
            }
            else
            {
                HPrefsSerialization.StringData.Add(key, value);
            }
        }

        #endregion string
    }
}
