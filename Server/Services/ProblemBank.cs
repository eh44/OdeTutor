// File: Server/Services/ProblemBank.cs
namespace Server.Services;

public static class ProblemBank
{
    // Maps a Module ID to a specific textbook problem and the AI's grading rubric
    public static readonly Dictionary<string, OdeProblem> Modules = new()
    {
        {
            "separable", new OdeProblem {
                Title = "Chapter 4: Separable Equations",
                ProblemLatex = "\\frac{dy}{dx} = 3xy^3, \\quad y(0)=\\frac{1}{2}",
                ExpertInstructions = "This is a separable differential equation. The student's first step must be separating the variables to get y terms on the left and x terms on the right: y^{-3} dy = 3x dx. If they integrate before separating, correct them."
            }
        },
        {
            "linear", new OdeProblem {
                Title = "Chapter 5: Linear First-Order Equations",
                ProblemLatex = "\\frac{dy}{dx} + 6xy = \\sin(x), \\quad y(0)=4",
                ExpertInstructions = "This is a first-order linear ODE. The student must identify the integrating factor \\mu(x) = e^{\\int 6x dx} = e^{3x^2}. The next step is multiplying the entire equation by this factor."
            }
        },
        {
            "substitution", new OdeProblem {
                Title = "Chapter 6: Simplifying Through Substitution",
                ProblemLatex = "\\frac{dy}{dx} = \\frac{y}{x} + \\left(\\frac{x}{y}\\right)^2",
                ExpertInstructions = "This is a homogeneous equation. Guide the student to use the substitution v = y/x. This means y = vx and dy/dx = v + x(dv/dx). Substituting this in makes the equation separable."
            }
        }
    };
}

public class OdeProblem
{
    public string Title { get; set; } = string.Empty;
    public string ProblemLatex { get; set; } = string.Empty;
    public string ExpertInstructions { get; set; } = string.Empty;
}