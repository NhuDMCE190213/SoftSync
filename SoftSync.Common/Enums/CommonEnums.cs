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
    Weak,
    Average,
    Good
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

public enum Gender
{
    Unspecified,
    Male,
    Female
}

public enum QuestionType
{
    MultipleChoice,
    Scenario
}

/// <summary>Self-reported current level, used by the AI to tune the roadmap.</summary>
public enum LearningLevel
{
    Unspecified,
    Beginner,
    Intermediate,
    Advanced
}

/// <summary>Preferred time of day to study.</summary>
public enum StudyTime
{
    Unspecified,
    Morning,
    Afternoon,
    Evening,
    Night
}

/// <summary>UI theme preference (System follows the OS setting).</summary>
public enum ThemePreference
{
    System,
    Light,
    Dark
}

/// <summary>What a <c>VerificationCode</c> is issued for.</summary>
public enum VerificationPurpose
{
    PhoneLoginOtp,
    PhoneNumberConfirmation,
    PasswordResetSms,
    PasswordResetEmail
}
