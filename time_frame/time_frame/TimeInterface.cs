using System;
using Hats.Time;
using System.Net.Http;

namespace Hats
{

namespace Time
{

public class TimeInterface
{
/**
 * Given the time to start the simulation at, sends a message to the configured client
 * \param[in] sim Simulated starting time
 * \param[in] rate Scaling factor for time flow
 * \param[in] client Client to send the time frame to
 * \param[out] Flag indicating success
 */
static public bool sendTimeFrame(DateTime sim, double rate, HttpClient client)
{
	return true;
}

/**
 * Send TimeFrame to the given client.
 * \param[in] frame TimeFrame to send out
 * \param[in] client Client to send TimeFrame to
 * \param[out] Flag indicating success
 */
static public bool sendTimeFrame(TimeFrame frame, HttpClient client)
{
	return true;
}

/**
 * Sets the TimeFrame from a string.
 * \param[out] frame TimeFrame to be set.
 * \param[in] msg TimeFrame as specified as a string.
 */
static public bool setTimeFrameFromString(TimeFrame frame, String msg)
{
	frame.setTimeFrame();
	return true;
}

}

}

}
