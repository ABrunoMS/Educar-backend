using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.AnswerTypes;

public static class AnswerTypeValidator
{
    public static bool ValidateAnswer(QuestionType? questionType, IAnswer? expectedAnswer)
    {
        if (expectedAnswer == null) return false;

        return questionType switch
        {
            QuestionType.MultipleChoice => ValidateMultipleChoice(expectedAnswer as MultipleChoice),
            QuestionType.TrueOrFalse => ValidateTrueOrFalse(expectedAnswer as TrueOrFalse),
            QuestionType.SingleChoice => ValidateSingleChoice(expectedAnswer as SingleChoice),
            QuestionType.Dissertative => ValidateDissertative(expectedAnswer as Dissertative),
            QuestionType.ColumnFill => ValidateColumnFill(expectedAnswer as ColumnFill),
            QuestionType.Ordering => ValidateOrdering(expectedAnswer as Ordering),
            QuestionType.MatchTwoRows => ValidateMatchTwoRows(expectedAnswer as MatchTwoRows),
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
        return singleChoice != null && !string.IsNullOrWhiteSpace(singleChoice.Option);
    }

    private static bool ValidateDissertative(Dissertative? dissertative)
    {
        // Ensure the expected text is not empty
        return dissertative != null && !string.IsNullOrWhiteSpace(dissertative.Text);
    }

    private static bool ValidateColumnFill(ColumnFill? columnFill)
    {
        if (columnFill?.Matches == null || columnFill.Matches.Count == 0)
            return false;

        // Ensure all keys and values in the dictionary are non-empty
        return columnFill.Matches.All(kv => !string.IsNullOrWhiteSpace(kv.Key) && !string.IsNullOrWhiteSpace(kv.Value));
    }

    private static bool ValidateOrdering(Ordering? ordering)
    {
        if (ordering == null) return false;
        var items = ordering.Items ?? new List<string>();
        var order = ordering.CorrectOrder ?? new List<int>();

        if (items.Count < 2) return false;

        if (order.Count != items.Count) return false;

        // Ensure correctOrder contains a permutation of 0..n-1
        var expected = Enumerable.Range(0, items.Count).ToList();
        var sorted = order.OrderBy(i => i).ToList();
        return sorted.SequenceEqual(expected);
    }

    private static bool ValidateMatchTwoRows(MatchTwoRows? match)
    {
        if (match == null) return false;
        var left = match.Left ?? new List<string>();
        var right = match.Right ?? new List<string>();
        var matches = match.Matches ?? new Dictionary<int, int>();

        if (left.Count == 0 || right.Count == 0) return false;

        // Each left item should have a mapping
        if (matches.Count != left.Count) return false;

        // Keys and values should be within bounds and values should be unique
        var rightValues = new HashSet<int>();
        foreach (var kv in matches)
        {
            if (kv.Key < 0 || kv.Key >= left.Count) return false;
            if (kv.Value < 0 || kv.Value >= right.Count) return false;
            rightValues.Add(kv.Value);
        }

        // Optionally enforce one-to-one mapping
        return rightValues.Count == matches.Count;
    }

    private static bool ValidateAlwaysCorrect(AlwaysCorrect? alwaysCorrect)
    {
        // AlwaysCorrect doesn't require validation as any answer is correct
        return alwaysCorrect != null;
    }
}