using System;
using System.Globalization;
using Architecture.Data.Structs;
using UnityEngine;
using UnityEngine.UI;

namespace HomeTools.Other
{
    // Additional method tools for app
    public static class OtherHTools
    {
        public static string CodingToBase64(string data)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(data);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string DecodingFromBase64(string data)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(data);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void SetRectTransformAnchorLeft(this RectTransform rect, float left)
        {
            rect.offsetMin = new Vector2(left, rect.offsetMin.y);
        }
        
        public static void SetRectTransformAnchorRight(this RectTransform rect, float right)
        {
            rect.offsetMax = new Vector2(-right, rect.offsetMax.y);
        }
        
        public static void SetRectTransformAnchorHorizontal(this RectTransform rect, float left, float right)
        {
            rect.offsetMax = new Vector2(-right, rect.offsetMax.y);
            rect.offsetMin = new Vector2(left, rect.offsetMin.y);
        }
        
        public static void SetRectTransformAnchorTop(this RectTransform rect, float top)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x, top);
        }
        
        public static void SetRectTransformAnchorBottom(this RectTransform rect, float bottom)
        {
            rect.offsetMin = new Vector2(rect.offsetMin.x, bottom);
        }
        
        public static void SetRectTransformAnchorVertical(this RectTransform rect, float top, float bottom)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x, top);
            rect.offsetMin = new Vector2(rect.offsetMin.x, bottom);
        }

        public static string ConvertToShortText(float num)
        {
            if (num < 1000)
                return num.ToString(CultureInfo.InvariantCulture);
            
            if (num < 1000000)
                return $"{(num/1000):0.0}" + "k";
            
            return $"{(num/1000000):0.0}" + "m";
        }
        
        public static string ConvertToSplitText(float num)
        {
            if (num < 1000)
                return num.ToString(CultureInfo.InvariantCulture);

            if (num < 1000000)
            {
                var result = string.Empty;
                var thousand = (int)(num / 1000);
                result += thousand;
                result += " ";
                result += (num - thousand * 1000).ToString("000");
                return result;
            }

            return (num/1000000).ToString(CultureInfo.InvariantCulture) + "m";
        }

        public static T[] ReturnChildren<T>(Transform parent) where T : Component
        {
            var result = new T[parent.childCount];
            
            for (var i = 0; i < result.Length; i++)
                result[i] = parent.GetChild(i).GetComponent<T>();

            return result;
        }

        public static HomeDay GetDayBySystem(DateTime dataTime) => new HomeDay(dataTime.Year, dataTime.Month, dataTime.Day);
        
        public static int DaysDistance(HomeDay firstDay, HomeDay secondDay)
        {
          var first = new DateTime(firstDay.Year, firstDay.Month, firstDay.Day);
          var second = new DateTime(secondDay.Year, secondDay.Month, secondDay.Day);
          
          return first.Subtract(second).Days;
        }

        public static int MonthDistance(HomeDay firstDay, HomeDay secondDay)
        {
            var result = 0;

            var first = new HomeDay(firstDay.Year, firstDay.Month, (byte) 1);
            var last = new HomeDay(secondDay.Year, secondDay.Month, (byte) 1);

            while (first > last)
            {
                first.AddMonths(-1);
                result++;
            }

            return result;
        }
        
        public static Vector2 GetWorldAnchoredPosition(RectTransform rectTransform)
        {
            var parent = rectTransform.parent;

            rectTransform.transform.SetParent(MainCanvas.RectTransform);
            var result = rectTransform.anchoredPosition;
            rectTransform.transform.SetParent(parent);

            return result;
        }
        
        public static void UpdateHeightOfText(Text text)
        {
            if (text == null)
                return;
            
            var textRt = text.rectTransform;
            var sizeDeltaRt = textRt.sizeDelta;
            sizeDeltaRt = new Vector2(sizeDeltaRt.x, 100000);
            
            var height = text.preferredHeight;
            
            sizeDeltaRt = new Vector2(sizeDeltaRt.x, height + 50);
            textRt.sizeDelta = sizeDeltaRt;
        }
        
        public static float DegreesToRadians(float degrees) => degrees * Mathf.PI / 180f;
    }
}
