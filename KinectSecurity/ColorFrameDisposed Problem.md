Starting in AllFramesReadyFrameSource.cs, function Sensor_AllFramesReady.

I have added an if statement:

if (colorFrame.Width != 0)
{
	newAlert.SendAlert(colorFrame);
}


which sends the captured color image frame to the EmailAlert function Send Alert.
This code sends an email with the color frame attached.

Back at the AllFramesReadyFrameSource.cs function Sensor_AllFramesReady, I have commented out colorFrame.Dispose();
this disposes of the frame that i am trying to send in an email before I can send it.
Not disposing of the frame is a bad thing, but it doesn't work otherwise.

I need help
