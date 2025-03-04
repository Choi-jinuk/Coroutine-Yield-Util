using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class YieldUtil
{
	class FloatComparer : IEqualityComparer<float>
	{
		bool IEqualityComparer<float>.Equals(float x, float y)
		{
			return x == y;
		}
		int IEqualityComparer<float>.GetHashCode(float obj)
		{
			return obj.GetHashCode();
		}
	}

	public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
	public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

	private static readonly Dictionary<float, WaitForSeconds> m_dicWaitForSeconds = new Dictionary<float, WaitForSeconds>(new FloatComparer());
	private static readonly Dictionary<float, List<WaitForSecondsRealTimeOptimized>> m_dicWaitForSecondsRealtimeOptimized = new Dictionary<float, List<WaitForSecondsRealTimeOptimized>>(new FloatComparer());

	public static WaitForSeconds WaitForSeconds(float seconds)
	{
		WaitForSeconds wfs;
		if (!m_dicWaitForSeconds.TryGetValue(seconds, out wfs))
			m_dicWaitForSeconds.Add(seconds, wfs = new WaitForSeconds(seconds));
		return wfs;
	}

  //WaitForSecondsRealTimeOptimized: realtimeSinceStartup + offsetTime 을 활용한 커스텀 Yield
	public static WaitForSecondsRealTimeOptimized WaitForSecondsRealTime(float seconds)
	{
		if (!m_dicWaitForSecondsRealtimeOptimized.TryGetValue(seconds, out var listWfsReal))
		{
			m_dicWaitForSecondsRealtimeOptimized.Add(seconds, listWfsReal = new List<WaitForSecondsRealTimeOptimized>());
		}

		WaitForSecondsRealTimeOptimized wfsReal = listWfsReal.Find(x => x.endWaiting == false);
		if (wfsReal == null)
		{
			wfsReal = new WaitForSecondsRealTimeOptimized(seconds);
			listWfsReal.Add(wfsReal);
		}
		
		wfsReal.RefreshTime(seconds);
		return wfsReal;
	}
}
