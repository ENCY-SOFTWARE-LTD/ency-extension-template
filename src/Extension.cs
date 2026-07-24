using System.Diagnostics;
using CAMAPI.Application;
using CAMAPI.DotnetHelper;
using CAMAPI.Extensions;
using CAMAPI.Project;
using CAMAPI.ResultStatus;

namespace EncyExtension;

/// <summary>
/// Example utility: dumps the active project's path and id into a text file and opens it.
/// Replace this with your extension logic.
/// </summary>
public class UtilityExtension : IExtension, IExtensionUtility
{
    /// <inheritdoc />
    public IExtensionInfo? Info { get; set; }

    /// <summary>
    /// Called when the user runs the utility from ENCY.
    /// </summary>
    /// <param name="context">Information about the current ENCY instance.</param>
    /// <param name="resultStatus">Error reporting (exceptions do not cross the host boundary).</param>
    public void Run(IExtensionUtilityContext context, out TResultStatus resultStatus)
    {
        resultStatus = default;
        try
        {
            using var projectCom = new ComWrapper<ICamApiProject>(
                context.CamApplication.GetActiveProject(out resultStatus));
            if (resultStatus.Code == TResultStatusCode.rsError)
                throw new Exception("Error getting project: " + resultStatus.Description);
            var project = projectCom.Instance
                ?? throw new Exception("No active project");

            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
            File.WriteAllText(tempFile,
                "Hello from EncyExtension!" + Environment.NewLine +
                "Project file path: " + project.FilePath + Environment.NewLine +
                "Project id: " + project.Id);
            Process.Start("notepad.exe", tempFile);
        }
        catch (Exception e)
        {
            resultStatus.Code = TResultStatusCode.rsError;
            resultStatus.Description = e.Message;
        }
    }
}
