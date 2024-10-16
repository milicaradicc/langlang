namespace LangLang.Models
{
    public class ExamGrade
    {
        public const int ReadingPointsMax = 60;
        public const int WritingPointsMax = 60;
        public const int ListeningPointsMax = 40;
        public const int TalkingPointsMax = 50;

        public const int MinPassingPoints = 160;

        private int _readingPoints;
        private int _writingPoints;
        private int _listeningPoints;
        private int _talkingPoints;
        private bool _passed;

        public ExamGrade(int id, int examId, int studentId, int readingPoints, int writingPoints, int listeningPoints, int talkingPoints)
        {
            Id = id;
            ExamId = examId;
            StudentId = studentId;
            ReadingPoints = readingPoints;
            WritingPoints = writingPoints;
            ListeningPoints = listeningPoints;
            TalkingPoints = talkingPoints;

            CalculatePassed();
        }

        public int Id { get; set; }

        public int ExamId { get; set; }

        public int StudentId { get; set; }

        public int ReadingPoints
        {
            get => _readingPoints;
            set
            {
                ValidateReadingPoints(value);
                _readingPoints = value;
            }
        }

        public int WritingPoints
        {
            get => _writingPoints;
            set
            {
                ValidateWritingPoints(value);
                _writingPoints = value;
            }
        }

        public int ListeningPoints
        {
            get => _listeningPoints;
            set
            {
                ValidateListeningPoints(value);
                _listeningPoints = value;
            }
        }

        public int TalkingPoints
        {
            get => _talkingPoints;
            set
            {
                ValidateTalkingPoints(value);
                _talkingPoints = value;
            }
        }

        public int PointsSum
        {
            get => _listeningPoints + _readingPoints + _talkingPoints + _writingPoints;
        }

        public bool Passed
        {
            get => _passed;
        }

        private static void ValidateReadingPoints(int readingPoints)
        {
            if (readingPoints < 0 || readingPoints > ReadingPointsMax)
            {
                throw new InvalidInputException("Reading points not valid");
            }
        }

        private static void ValidateWritingPoints(int writingPoints)
        {
            if (writingPoints < 0 || writingPoints > WritingPointsMax)
            {
                throw new InvalidInputException("Writing points not valid");
            }
        }

        private static void ValidateListeningPoints(int listeningPoints)
        {
            if (listeningPoints < 0 || listeningPoints > ListeningPointsMax)
            {
                throw new InvalidInputException("Listening points not valid");
            }
        }

        private static void ValidateTalkingPoints(int talkingPoints)
        {
            if (talkingPoints < 0 || talkingPoints > TalkingPointsMax)
            {
                throw new InvalidInputException("Talking points not valid");
            }
        }

        private void CalculatePassed()
        {
            if (_readingPoints < ReadingPointsMax / 2 ||
                _writingPoints < WritingPointsMax / 2 ||
                _listeningPoints < ListeningPointsMax / 2 ||
                _talkingPoints < TalkingPointsMax / 2 ||
                _readingPoints + _writingPoints + _listeningPoints + _talkingPoints < MinPassingPoints)
            {
                _passed = false;
                return;
            }

            _passed = true;
        }
    }
}
