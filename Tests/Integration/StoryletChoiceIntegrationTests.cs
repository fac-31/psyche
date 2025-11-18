namespace Psyche.Tests.Integration;

using Psyche.Models;
using Psyche.Models.Mocks;
using Psyche.Systems.Prerequisites;
using Psyche.Systems.Effects;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

public class StoryletChoiceIntegrationTests
{
    private readonly ITestOutputHelper _output;

    public StoryletChoiceIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void LegacyStorylet_WithoutOptions_AppliesEffectsAutomatically()
    {
        // Arrange
        TestOutputHelpers.LogTestHeader(_output, "Legacy Storylet Behavior (No Choices)");

        var character = new Character();
        character.Attributes.Bravery = 50;

        var storylet = new Storylet
        {
            Id = "legacy_encounter",
            Title = "A Brave Encounter",
            Description = "You face danger head-on",
            Content = "Your courage is tested...",
            Effects = new List<IEffect>
            {
                new AttributeEffect { AttributeName = "Bravery", Delta = 5 }
            }
        };

        TestOutputHelpers.LogCharacterState(_output, character, "Before storylet");

        // Act
        foreach (var effect in storylet.Effects)
        {
            effect.Apply(character);
        }

        // Assert
        character.Attributes.Bravery.Should().Be(55);
        storylet.HasChoices.Should().BeFalse();

        TestOutputHelpers.LogCharacterState(_output, character, "After storylet");
        _output.WriteLine($"\n✓ Legacy storylet applied effects automatically");
    }

    [Fact]
    public void SimpleChoiceStorylet_PlayerChoosesOption_EffectsApply()
    {
        // Arrange
        TestOutputHelpers.LogTestHeader(_output, "Simple Two-Choice Storylet");

        var character = new Character();
        character.Attributes.Compassion = 50;
        character.Qualities["social_capital"] = 10;

        var storylet = new Storylet
        {
            Id = "moral_dilemma",
            Title = "A Moral Dilemma",
            Description = "You encounter someone in need",
            Content = "A stranger asks for help. Will you assist them?",
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "help",
                    Text = "Help them",
                    ResultText = "You offer your assistance...",
                    Effects = new List<IEffect>
                    {
                        new AttributeEffect { AttributeName = "Compassion", Delta = 5 },
                        new QualityEffect { QualityId = "social_capital", Delta = 3 }
                    }
                },
                new StoryletOption
                {
                    Id = "ignore",
                    Text = "Ignore them",
                    ResultText = "You walk past...",
                    Effects = new List<IEffect>
                    {
                        new AttributeEffect { AttributeName = "Compassion", Delta = -2 }
                    }
                }
            }
        };

        TestOutputHelpers.LogCharacterState(_output, character, "Before choice");
        _output.WriteLine($"\nStorylet: {storylet.Title}");
        _output.WriteLine($"Content: {storylet.Content}\n");

        // Act - Player chooses to help
        var availableOptions = storylet.GetAvailableOptions(character);
        availableOptions.Should().HaveCount(2, "both options should be available");

        var chosenOption = availableOptions.First(opt => opt.Id == "help");
        _output.WriteLine($"Player chooses: {chosenOption.Text}");
        _output.WriteLine($"Result: {chosenOption.ResultText}\n");

        foreach (var effect in chosenOption.Effects)
        {
            _output.WriteLine($"  {effect.GetDisplayText()}");
            effect.Apply(character);
        }

        // Assert
        character.Attributes.Compassion.Should().Be(55);
        character.Qualities["social_capital"].Should().Be(13);

        TestOutputHelpers.LogCharacterState(_output, character, "After choice");
    }

    [Fact]
    public void GatedChoices_OnlyAvailableOptionsShown()
    {
        // Arrange
        TestOutputHelpers.LogTestHeader(_output, "Gated Choices Based on Attributes");

        var character = new Character();
        character.Attributes.Bravery = 70;
        character.Attributes.Compassion = 40;
        character.Attributes.Discernment = 75;

        var storylet = new Storylet
        {
            Id = "complex_decision",
            Title = "A Complex Decision",
            Content = "Multiple paths lie before you, but not all are open...",
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "brave_path",
                    Text = "Take the brave path",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 }
                    },
                    Effects = new List<IEffect>
                    {
                        new AttributeEffect { AttributeName = "Bravery", Delta = 5 }
                    }
                },
                new StoryletOption
                {
                    Id = "compassionate_path",
                    Text = "Take the compassionate path",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new AttributeRequirement { AttributeName = "Compassion", MinValue = 60 }
                    },
                    Effects = new List<IEffect>
                    {
                        new AttributeEffect { AttributeName = "Compassion", Delta = 5 }
                    }
                },
                new StoryletOption
                {
                    Id = "discerning_path",
                    Text = "Take the discerning path",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new AttributeRequirement { AttributeName = "Discernment", MinValue = 70 }
                    },
                    Effects = new List<IEffect>
                    {
                        new AttributeEffect { AttributeName = "Discernment", Delta = 5 }
                    }
                }
            }
        };

        TestOutputHelpers.LogCharacterState(_output, character, "Character attributes");

        // Act
        var availableOptions = storylet.GetAvailableOptions(character);

        // Assert
        availableOptions.Should().HaveCount(2, "only brave and discerning paths meet prerequisites");
        availableOptions.Should().Contain(opt => opt.Id == "brave_path");
        availableOptions.Should().Contain(opt => opt.Id == "discerning_path");
        availableOptions.Should().NotContain(opt => opt.Id == "compassionate_path");

        _output.WriteLine($"\nAvailable options:");
        foreach (var option in availableOptions)
        {
            _output.WriteLine($"  ✓ {option.Text}");
        }
        _output.WriteLine($"\nLocked options:");
        var lockedOptions = storylet.Options.Except(availableOptions);
        foreach (var option in lockedOptions)
        {
            _output.WriteLine($"  ✗ {option.Text}");
        }
    }

    [Fact]
    public void StoryletAndOptionEffects_BothApply()
    {
        // Arrange
        TestOutputHelpers.LogTestHeader(_output, "Storylet-Level and Option-Level Effects");

        var character = new Character();
        character.Attributes.Bravery = 50;
        character.Qualities["psychological_strain"] = 20;

        var storylet = new Storylet
        {
            Id = "dangerous_situation",
            Title = "A Dangerous Situation",
            Content = "You find yourself in peril...",
            Effects = new List<IEffect>
            {
                // Storylet-level effect applied on view
                new QualityEffect { QualityId = "psychological_strain", Delta = 5 }
            },
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "face_danger",
                    Text = "Face the danger",
                    ResultText = "You steel yourself and act...",
                    Effects = new List<IEffect>
                    {
                        new AttributeEffect { AttributeName = "Bravery", Delta = 10 }
                    }
                }
            }
        };

        TestOutputHelpers.LogCharacterState(_output, character, "Initial state");

        // Act - Apply storylet-level effects (on view)
        _output.WriteLine($"\n1. Viewing storylet '{storylet.Title}':");
        foreach (var effect in storylet.Effects)
        {
            _output.WriteLine($"   {effect.GetDisplayText()}");
            effect.Apply(character);
        }

        TestOutputHelpers.LogCharacterState(_output, character, "After viewing");

        // Act - Apply option effects (on choice)
        var chosenOption = storylet.Options[0];
        _output.WriteLine($"\n2. Choosing option '{chosenOption.Text}':");
        foreach (var effect in chosenOption.Effects)
        {
            _output.WriteLine($"   {effect.GetDisplayText()}");
            effect.Apply(character);
        }

        // Assert
        character.Attributes.Bravery.Should().Be(60);
        character.Qualities["psychological_strain"].Should().Be(25);

        TestOutputHelpers.LogCharacterState(_output, character, "Final state");
    }

    [Fact]
    public void ComplexScenario_CompoundPrerequisitesAndEffects()
    {
        // Arrange
        TestOutputHelpers.LogTestHeader(_output, "Complex Multi-Quality Scenario");

        var character = new Character();
        character.Attributes.Bravery = 65;
        character.Attributes.Discernment = 55;
        character.Qualities["social_capital"] = 8;
        character.Qualities["psychological_strain"] = 40;
        character.Qualities["main_story_progress"] = 1;

        var storylet = new Storylet
        {
            Id = "pivotal_moment",
            Title = "A Pivotal Moment",
            Content = "Everything comes to a head. Your choices matter.",
            Prerequisites = new List<IPrerequisite>
            {
                new QualityRequirement { QualityId = "main_story_progress", MinValue = 1 }
            },
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "bold_action",
                    Text = "Take bold action",
                    Description = "Requires bravery OR discernment, and manageable stress",
                    ResultText = "You act decisively...",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new CompoundPrerequisite
                        {
                            Logic = CompoundLogic.And,
                            Prerequisites = new List<IPrerequisite>
                            {
                                new CompoundPrerequisite
                                {
                                    Logic = CompoundLogic.Or,
                                    Prerequisites = new List<IPrerequisite>
                                    {
                                        new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
                                        new AttributeRequirement { AttributeName = "Discernment", MinValue = 70 }
                                    }
                                },
                                new QualityRequirement { QualityId = "psychological_strain", MaxValue = 50 }
                            }
                        }
                    },
                    Effects = new List<IEffect>
                    {
                        new CompoundEffect
                        {
                            Effects = new List<IEffect>
                            {
                                new AttributeEffect { AttributeName = "Bravery", Delta = 5 },
                                new QualityEffect { QualityId = "psychological_strain", Delta = 10 },
                                new QualityEffect { QualityId = "main_story_progress", Delta = 1 },
                                new QualityEffect { QualityId = "social_capital", Delta = -3 }
                            }
                        }
                    }
                },
                new StoryletOption
                {
                    Id = "cautious_approach",
                    Text = "Take a cautious approach",
                    ResultText = "You proceed carefully...",
                    Effects = new List<IEffect>
                    {
                        new AttributeEffect { AttributeName = "Discernment", Delta = 3 },
                        new QualityEffect { QualityId = "psychological_strain", Delta = -5 }
                    }
                }
            }
        };

        TestOutputHelpers.LogCharacterState(_output, character, "Initial state");
        _output.WriteLine($"\nEvaluating prerequisites for '{storylet.Title}':");

        // Check storylet prerequisites
        var storyletAvailable = storylet.Prerequisites.All(prereq => prereq.IsMet(character));
        _output.WriteLine($"Storylet available: {(storyletAvailable ? "✓" : "✗")}");

        // Act
        var availableOptions = storylet.GetAvailableOptions(character);

        // Assert
        storyletAvailable.Should().BeTrue();
        availableOptions.Should().HaveCount(2, "both options should be available");

        _output.WriteLine($"\nAvailable options:");
        foreach (var option in availableOptions)
        {
            _output.WriteLine($"  ✓ {option.Text}");
        }

        // Act - Choose bold action
        var chosenOption = availableOptions.First(opt => opt.Id == "bold_action");
        _output.WriteLine($"\nPlayer chooses: {chosenOption.Text}");
        _output.WriteLine($"{chosenOption.ResultText}\n");

        _output.WriteLine("Applying effects:");
        foreach (var effect in chosenOption.Effects)
        {
            _output.WriteLine($"  {effect.GetDisplayText()}");
            effect.Apply(character);
        }

        // Assert final state
        character.Attributes.Bravery.Should().Be(70);
        character.Qualities["psychological_strain"].Should().Be(50);
        character.Qualities["main_story_progress"].Should().Be(2);
        character.Qualities["social_capital"].Should().Be(5);

        TestOutputHelpers.LogCharacterState(_output, character, "Final state");
    }

    [Fact]
    public void AllOptionsLocked_NoChoicesAvailable()
    {
        // Arrange
        TestOutputHelpers.LogTestHeader(_output, "All Options Locked Scenario");

        var character = new Character();
        character.Attributes.Bravery = 30;
        character.Attributes.Compassion = 35;

        var storylet = new Storylet
        {
            Id = "locked_storylet",
            Title = "Beyond Your Reach",
            Content = "You see paths forward, but none are within your grasp...",
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "brave_option",
                    Text = "Brave approach",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 }
                    }
                },
                new StoryletOption
                {
                    Id = "compassionate_option",
                    Text = "Compassionate approach",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new AttributeRequirement { AttributeName = "Compassion", MinValue = 60 }
                    }
                }
            }
        };

        TestOutputHelpers.LogCharacterState(_output, character, "Character state");

        // Act
        var availableOptions = storylet.GetAvailableOptions(character);

        // Assert
        availableOptions.Should().BeEmpty();

        _output.WriteLine($"\n✗ No options available - all prerequisites failed");
        _output.WriteLine($"This storylet should either:");
        _output.WriteLine($"  1. Not be shown to the player (storylet-level prerequisites)");
        _output.WriteLine($"  2. Show locked options with hints to unlock them");
    }

    [Fact]
    public void OptionUnlockingChain_ChoiceUnlocksNextStorylet()
    {
        // Arrange
        TestOutputHelpers.LogTestHeader(_output, "Option Choice Unlocks Future Storylet");

        var character = new Character();
        character.Attributes.Compassion = 60;

        var firstStorylet = new Storylet
        {
            Id = "first_encounter",
            Title = "First Encounter",
            Content = "You meet someone new...",
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "befriend",
                    Text = "Befriend them",
                    ResultText = "You make a new friend...",
                    Effects = new List<IEffect>
                    {
                        new AttributeEffect { AttributeName = "Compassion", Delta = 3 },
                        new UnlockStoryletEffect { StoryletId = "first_encounter" }, // Mark as played
                        new QualityEffect { QualityId = "social_capital", Delta = 5 }
                    }
                }
            }
        };

        var followUpStorylet = new Storylet
        {
            Id = "reunion",
            Title = "Reunion",
            Content = "You meet your friend again...",
            Prerequisites = new List<IPrerequisite>
            {
                new StoryletPlayedRequirement { StoryletId = "first_encounter", MustHavePlayed = true }
            }
        };

        TestOutputHelpers.LogCharacterState(_output, character, "Initial state");
        _output.WriteLine($"\nFirst storylet: {firstStorylet.Title}");

        // Act - Play first storylet
        var option = firstStorylet.Options[0];
        _output.WriteLine($"Player chooses: {option.Text}");

        foreach (var effect in option.Effects)
        {
            _output.WriteLine($"  {effect.GetDisplayText()}");
            effect.Apply(character);
        }

        TestOutputHelpers.LogCharacterState(_output, character, "After first storylet");

        // Check if follow-up is now available
        var followUpAvailable = followUpStorylet.Prerequisites.All(prereq => prereq.IsMet(character));

        // Assert
        followUpAvailable.Should().BeTrue();
        character.HasPlayedStorylet("first_encounter").Should().BeTrue();
        character.Qualities["social_capital"].Should().Be(5);

        _output.WriteLine($"\n✓ Follow-up storylet '{followUpStorylet.Title}' is now available");
    }
}
