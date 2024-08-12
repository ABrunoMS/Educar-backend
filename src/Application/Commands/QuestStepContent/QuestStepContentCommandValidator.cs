using Educar.Backend.Application.Commands.QuestStepContent.ExpectedAnswerTypes;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent;

public static class QuestStepContentCommandValidator
{
    public static bool ValidateExpectedAnswer(QuestionType? questionType, IExpectedAnswer? expectedAnswer)
    {
        if (expectedAnswer == null) return false;

        return questionType switch
        {
            QuestionType.MultipleChoice => ValidateMultipleChoice(expectedAnswer as MultipleChoice),
            QuestionType.TrueOrFalse => ValidateTrueOrFalse(expectedAnswer as TrueOrFalse),
            QuestionType.SingleChoice => ValidateSingleChoice(expectedAnswer as SingleChoice),
            QuestionType.Dissertative => ValidateDissertative(expectedAnswer as Dissertative),
            QuestionType.ColumnFill => ValidateColumnFill(expectedAnswer as ColumnFill),
            QuestionType.AlwaysCorrect => ValidateAlwaysCorrect(expectedAnswer as AlwaysCorrect),
            _ => false,
        };
    }

    private static bool ValidateMultipleChoice(MultipleChoice? multipleChoice)
    {
        if (multipleChoice == null || multipleChoice.Options == null || !multipleChoice.Options.Any())
            return false;

        // Ensure each option has a non-empty description
        return !multipleChoice.Options.Any(option => string.IsNullOrWhiteSpace(option.Description)) &&
               // Ensure there's at least one correct option
               multipleChoice.Options.Any(option => option.IsCorrect);
    }

    private static bool ValidateTrueOrFalse(TrueOrFalse? trueOrFalse)
    {
        // TrueOrFalse is simple, just ensure it's not null
        return trueOrFalse != null;
    }

    private static bool ValidateSingleChoice(SingleChoice? singleChoice)
    {
        // Ensure the correct option is not empty
        return singleChoice != null && !string.IsNullOrWhiteSpace(singleChoice.CorrectOption);
    }

    private static bool ValidateDissertative(Dissertative? dissertative)
    {
        // Ensure the expected text is not empty
        return dissertative != null && !string.IsNullOrWhiteSpace(dissertative.ExpectedText);
    }

    private static bool ValidateColumnFill(ColumnFill? columnFill)
    {
        if (columnFill?.Matches == null || columnFill.Matches.Count == 0)
            return false;

        // Ensure all keys and values in the dictionary are non-empty
        return columnFill.Matches.All(kv => !string.IsNullOrWhiteSpace(kv.Key) && !string.IsNullOrWhiteSpace(kv.Value));
    }

    private static bool ValidateAlwaysCorrect(AlwaysCorrect? alwaysCorrect)
    {
        // AlwaysCorrect doesn't require validation as any answer is correct
        return alwaysCorrect != null;
    }
}