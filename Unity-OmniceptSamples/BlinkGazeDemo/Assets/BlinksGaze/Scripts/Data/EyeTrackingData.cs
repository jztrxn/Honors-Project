using HP.Omnicept.Messaging.Messages;

public class EyeTrackingData
{
    //string[] headers = 
    //{
    //    "ts/hw",
    //    "ts/sys",
    //    "ts/omni",
    //    "sensor/dev/id",
    //    "sensor/dev/sub",
    //    "sensor/loc",
    //    "cgaze/x",
    //    "cgaze/y",
    //    "cgaze/z",
    //    "cgaze/q",
    //    "left/gaze/x",
    //    "left/gaze/y",
    //    "left/gaze/z",
    //    "left/gaze/q",
    //    "left/openness",
    //    "left/openness_q",
    //    "left/dilation",
    //    "left/dilation_q",
    //    "left/ppos/x",
    //    "left/ppos/y",
    //    "left/ppos/q",
    //    "right/gaze/x",
    //    "right/gaze/y",
    //    "right/gaze/z",
    //    "right/gaze/q",
    //    "right/openness",
    //    "right/openness_q",
    //    "right/dilation",
    //    "right/dilation_q",
    //    "right/ppos/x",
    //    "right/ppos/y",
    //    "right/ppos/q"
    //};

    public long ts_hw { get; set; }
    public long ts_sys { get; set; }
    public long ts_omni { get; set; }

    public string sensor_dev_id { get; set; }
    public string sensor_dev_sub { get; set; }
    public string sensor_loc { get; set; }

    public float cgaze_x { get; set; }
    public float cgaze_y { get; set; }
    public float cgaze_z { get; set; }
    public int cgaze_q { get; set; }

    public float left_gaze_x { get; set; }
    public float left_gaze_y { get; set; }
    public float left_gaze_z { get; set; }
    public int left_gaze_q { get; set; }

    public int left_openness { get; set; }
    public int left_openness_q { get; set; }

    public float left_dilation { get; set; }
    public int left_dilation_q { get; set; }

    public float left_ppos_x { get; set; }
    public float left_ppos_y { get; set; }
    public int left_ppos_q { get; set; }

    public float right_gaze_x { get; set; }
    public float right_gaze_y { get; set; }
    public float right_gaze_z { get; set; }
    public int right_gaze_q { get; set; }

    public int right_openness { get; set; }
    public int right_openness_q { get; set; }

    public float right_dilation { get; set; }
    public int right_dilation_q { get; set; }

    public float right_ppos_x { get; set; }
    public float right_ppos_y { get; set; }
    public float right_ppos_q { get; set; }

    public void From(EyeTracking eyeTracking)
    {
        if (eyeTracking == null) return;

        sensor_dev_id = eyeTracking.SensorInfo.DeviceID.ID;
        sensor_dev_sub = eyeTracking.SensorInfo.DeviceID.SubID;
        sensor_loc = eyeTracking.SensorInfo.Location;

        // left eye
        left_gaze_x = eyeTracking.LeftEye.Gaze.X;
        left_gaze_y = eyeTracking.LeftEye.Gaze.Y;
        left_gaze_z = eyeTracking.LeftEye.Gaze.Z;
        left_gaze_q = (int)eyeTracking.LeftEye.Gaze.Confidence;

        left_openness = (int)eyeTracking.LeftEye.Openness;
        left_openness_q = (int)eyeTracking.LeftEye.OpennessConfidence;

        left_dilation = eyeTracking.LeftEye.PupilDilation;
        left_dilation_q = (int)eyeTracking.LeftEye.PupilDilationConfidence;

        left_ppos_x = eyeTracking.LeftEye.PupilPosition.X;
        left_ppos_y = eyeTracking.LeftEye.PupilPosition.Y;
        left_ppos_q = (int)eyeTracking.LeftEye.PupilPosition.Confidence;

        // right eye
        right_gaze_x = eyeTracking.RightEye.Gaze.X;
        right_gaze_y = eyeTracking.RightEye.Gaze.Y;
        right_gaze_z = eyeTracking.RightEye.Gaze.Z;
        right_gaze_q = (int)eyeTracking.RightEye.Gaze.Confidence;

        right_openness = (int)eyeTracking.RightEye.Openness;
        right_openness_q = (int)eyeTracking.RightEye.OpennessConfidence;

        right_dilation = eyeTracking.RightEye.PupilDilation;
        right_dilation_q = (int)eyeTracking.RightEye.PupilDilationConfidence;

        right_ppos_x = eyeTracking.RightEye.PupilPosition.X;
        right_ppos_y = eyeTracking.RightEye.PupilPosition.Y;
        right_ppos_q = (int)eyeTracking.RightEye.PupilPosition.Confidence;
    }
}
