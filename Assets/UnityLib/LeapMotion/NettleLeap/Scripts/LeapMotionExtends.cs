using Leap;

namespace Nettle.NettleLeap {

public static class LeapMotionHandExtend {

    public static bool IsOnlyIndexExtended(this Hand hand) {

        return hand.Fingers[1].IsExtended &&
               //!hand.Fingers[0].IsExtended &&
               !hand.Fingers[2].IsExtended &&
               !hand.Fingers[3].IsExtended &&
               !hand.Fingers[4].IsExtended;

    }

    public static bool AllFingersExtended(this Hand hand) {

        return hand.Fingers[0].IsExtended &&
               hand.Fingers[1].IsExtended &&
               hand.Fingers[2].IsExtended &&
               hand.Fingers[3].IsExtended &&
               hand.Fingers[4].IsExtended;

    }

}
}
