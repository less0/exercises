using bowling_backend_core.DomainModel;

namespace bowling_backend_applicaton
{
    public class FrameDto
    {
        public int[] Rolls { get; init; }
        public int Score { get; init; }
        public int CumulativeScore { get; internal set; }

        public static explicit operator FrameDto(Frame frame)
        {
            return new FrameDto()
            {
                Rolls = frame.Rolls,
                Score = frame.Score, 
            };
        }
    }
}