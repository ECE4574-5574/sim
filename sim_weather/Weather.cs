using System;

namespace Hats.SimWeather
{

/**
 * Interface for accessing the weather for a given house.
 */
public interface IWeather
{
	/**
	 * Gets the temperature at the current time, according to the given TimeFrame.
	 * \param[out] Current temperature in Celsius, NaN if no setpoints exist
	 */
	Double Temperature();
	/**
	 * Gets the temperature at the given time. Will convert the given time to the
	 * internal TimeFrame if requested, otherwise assumes the given time is valid in that TimeFrame.
	 * \param[in] now Time to get temperature for.
	 * \param[in] convertTime Flag indicating if time should be converted.
	 * \param[out] Temperature in Celsius
	 */
	Double Temperature(DateTime now, bool convert_time = false);
};

}

