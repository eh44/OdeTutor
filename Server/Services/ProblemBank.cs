using System.Collections.Generic;

namespace Server.Services;

public class ModuleContext
{
    public string Title { get; set; } = string.Empty;
    public string TextbookExcerpt { get; set; } = string.Empty;
}

public static class ProblemBank
{
    public static readonly Dictionary<string, ModuleContext> Modules = new()
    {
        {
            "separable", new ModuleContext
            {
                Title = "Separable Equations (Chapter 4)",
                TextbookExcerpt = @"
                    TEXTBOOK METHODOLOGY: SEPARABLE EQUATIONS
                    1. A differential equation is separable if it can be written as dy/dx = g(x)h(y).
                    2. Separate the variables by multiplying/dividing so all y terms (including dy) are on one side and all x terms (including dx) are on the other: (1/h(y))dy = g(x)dx.
                    3. Integrate both sides: \int (1/h(y)) dy = \int g(x) dx.
                    4. Add a single arbitrary constant of integration (+C) to the independent variable side.
                    5. If an initial condition is provided, substitute the values to solve for C.
                    6. Isolate y to find the explicit solution, if algebraically possible."
            }
        },
        {
            "linear", new ModuleContext
            {
                Title = "First-Order Linear Equations (Chapter 5)",
                TextbookExcerpt = @"
                    TEXTBOOK METHODOLOGY: LINEAR EQUATIONS
                    1. Write the equation in standard form: dy/dx + P(x)y = Q(x). Ensure the coefficient of dy/dx is exactly 1.
                    2. Identify the function P(x).
                    3. Calculate the Integrating Factor: \mu(x) = e^{\int P(x)dx}. (Do not add a +C during this step).
                    4. Multiply the entire standard form equation by \mu(x).
                    5. Rewrite the left side as the derivative of a product: \frac{d}{dx}[\mu(x)y] = \mu(x)Q(x).
                    6. Integrate both sides with respect to x. Remember to add +C on the right side.
                    7. Divide by \mu(x) to isolate y.
                    8. Apply initial conditions if given to solve for C."
            }
        },
        {
            "substitution", new ModuleContext
            {
                Title = "Simplifying Through Substitution (Chapter 6)",
                TextbookExcerpt = @"
                    TEXTBOOK METHODOLOGY: SUBSTITUTION (HOMOGENEOUS)
                    1. If the equation can be written in the form dy/dx = F(y/x), it is homogeneous.
                    2. Use the substitution u = y/x, which means y = ux.
                    3. Differentiate using the product rule: dy/dx = u + x(du/dx).
                    4. Substitute y and dy/dx into the original equation. This will always result in a Separable equation in terms of u and x.
                    5. Separate the variables u and x, and integrate both sides.
                    6. Substitute u = y/x back into the equation to return to the original variables."
            }
        }
    };
}