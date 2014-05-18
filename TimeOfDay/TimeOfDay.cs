using UnityEngine;
using System.Collections;
 
public static class TimeOfDay 
{	
	// Day duration in seconds.
	public static float dayDuration{ get{ return m_fDayDuration; } set{ m_fDayDuration = value; m_fDayDurationInv = 1f / m_fDayDuration; } }
	private static float m_fDayDuration = 10f;
	private static float m_fDayDurationInv = 0.1F;
	public static bool useRealTime = false;
 
	private static int m_iDays;
	private static int m_iHours;
	private static int m_iMinutes;
	private static int m_iSeconds;
	private static float m_fPercentage;
 
	private const int MINUTES_IN_ONE_HOUR = 60;
	private const int SECONDS_IN_ONE_HOUR = MINUTES_IN_ONE_HOUR*60;
	private const int SECONDS_IN_ONE_DAY = SECONDS_IN_ONE_HOUR*24;
 
	public static void Sample() 
	{
		float t = useRealTime ? Time.realtimeSinceStartup : Time.time;
		m_fPercentage = t % m_fDayDuration * m_fDayDurationInv;
 
		m_iSeconds = Mathf.RoundToInt( m_fPercentage * SECONDS_IN_ONE_DAY );
		m_iDays = Mathf.FloorToInt( t * m_fDayDurationInv );
		m_iHours = m_iSeconds / SECONDS_IN_ONE_HOUR;
		m_iMinutes = (m_iSeconds % SECONDS_IN_ONE_HOUR) / MINUTES_IN_ONE_HOUR;
		m_iSeconds = (m_iSeconds % SECONDS_IN_ONE_HOUR) % MINUTES_IN_ONE_HOUR;
	}
 
	public static string GetTimeOfDay(){ return GetTimeOfDay("{0:00}:{1:00}:{2:00}"); }
	public static string GetTimeOfDay( string format )
	{
		Sample();
		return string.Format ( format, m_iHours, m_iMinutes, m_iSeconds );
	}
 
	public static int Days(){ Sample(); return m_iDays; }
	public static int Hours(){ Sample(); return m_iHours; }
	public static int Minutes(){ Sample(); return m_iMinutes; }
	public static int Seconds(){ Sample(); return m_iSeconds; }
	public static float Percentage(){ Sample(); return m_fPercentage; }
	// return 0 at midnight, 1 at noon and back at 0 at midnight
	public static float PercentagePingPong(){ return Mathf.PingPong( Percentage() * 2, 1f ); }
}