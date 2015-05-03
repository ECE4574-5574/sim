/**
 * Class for using simulated or real time in a consistent fashion.
 * \author: Jason Ziglar <jpz@vt.edu>
 */
using System;

namespace Hats
{

namespace Time
{

public class TimeFrame
{
	public TimeFrame(DateTime wall, DateTime sim, double rate)
	{
		setTimeFrame(wall, sim, rate);
	}

	public TimeFrame()
	{
		setTimeFrame();
	}

	/**
	 * Function which returns the time which should be treated as now. If under sim, computes simulated time.
	 */ 
	public DateTime now()
	{
		return time(DateTime.UtcNow);
	}

	/**
	 * Function which converts a time into the relative frame.
	 * \param[in] real_time Wall time to convert into the real time frame.
	 * \param[out] DateTime converted into the new time frame.
	 */
	public DateTime time(DateTime real_time)
	{
		TimeSpan diff = real_time.ToUniversalTime() - _wall_epoch;
		double scaled_ticks = (double)diff.Ticks * Rate;
		TimeSpan final_diff = TimeSpan.FromTicks((long)scaled_ticks);
		return (_sim_epoch + final_diff);
	}

	/**
	 * Sets the time frame for referencing a common time frame, even in the presence of simulation.
	 * \param[in] wall The epoch of this time frame, defined by the wall clock in the UTC time zone.
	 * \param[in] sim The epoch of this time frame in the simulated time frame.
	 * \param[in] rate The scaling factor between wall clock time and this time frame.
	 */
	public void setTimeFrame(DateTime wall = default(DateTime), DateTime sim = default(DateTime), double rate = 1.0)
	{
		if(rate <= 0.0)
		{
			throw new ArgumentException("Time rate must be greater than zero.", "rate");
		}
		if(wall == DateTime.MinValue)
		{
			wall = DateTime.UtcNow;
		}
		if(sim == DateTime.MinValue)
		{
			sim = DateTime.UtcNow;
		}
		_wall_epoch = wall;
		_sim_epoch = sim;
		_wall_epoch.ToUniversalTime();
		_sim_epoch.ToUniversalTime();
		Rate = rate;
	}

	/**
     * Function for accessing the definition of a time frame.
     * \param[out] wall The epoch of this time frame, defined by the wall clock in the UTC time zone.
     * \param[out] sim The epoch of this time frame in the simulated time frame.
     * \param[out] rate The scaling factor between wall clock time and this time frame.
     */
	public void getTimeFrame(out DateTime wall, out DateTime sim, out double rate)
	{
		wall = _wall_epoch;
		sim = _sim_epoch;
		rate = Rate;
	}

	public DateTime WallEpoch
	{
		get
		{
			return _wall_epoch;
		}
		set
		{
			_wall_epoch = value;
			_wall_epoch.ToUniversalTime();
		}
	}

	public DateTime SimEpoch
	{
		get
		{
			return _sim_epoch;
		}
		set
		{
			_sim_epoch = value;
			_sim_epoch.ToUniversalTime();
		}
	}

	public double Rate { get; set; }

	protected DateTime _wall_epoch;
	protected DateTime _sim_epoch;

	}

}

}