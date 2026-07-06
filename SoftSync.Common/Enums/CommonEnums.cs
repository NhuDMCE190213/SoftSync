namespace SoftSync.Common.Enums;

public enum SkillType
{
    Communication,
    Teamwork,
    TimeManagement,
    CriticalThinking,
    ProblemSolving,
    EmotionalManagement,
    Adaptability
}

public enum AssessmentLevel
{
    Passive,       // Bị động (24-41 tổng / 8-14 mỗi trụ)
    Developing,    // Đang phát triển (42-59 / 15-20)
    Proactive,     // Chủ động (60-77 / 21-26)
    Exceptional    // Làm chủ (78-96 / 27-32)
}

// Loại mini game, vì mỗi game cần UI/luồng chơi khác nhau
public enum MiniGameType
{
    ChooseBestAnswer,   // Game 1: chọn câu trả lời phù hợp
    FixTheSentence,     // Game 2: sửa câu giao tiếp
    MatchPairs,         // Game 3: ghép câu với tình huống
    AiRolePlay,         // Game 4: chat role-play với AI
    VoicePractice,      // Game 5: thực hành giọng nói
    DragAndSort,        // Priority Sort, Eisenhower Matrix (QLTG)
    Timetable           // My Timetable (check-in ảnh theo giờ)
}

public enum ChatSender
{
    User,
    Assistant
}

public enum UserRole
{
    Student
}

public enum QuestionType
{
    MultipleChoice,
    Scenario
}
