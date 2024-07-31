<Query Kind="Program" />

/// <summary>Provides utility methods for file IO.</summary>
public static class FileHelper
{
	/*********
	** Public methods
	*********/
	/// <summary>Delete a file or folder regardless of file permissions, and block until deletion completes.</summary>
	/// <param name="entry">The file or folder to reset.</param>
	/// <remarks>This method is mirred from <c>FileUtilities.ForceDelete</c> in the toolkit.</remarks>
	public static void ForceDelete(FileSystemInfo entry)
	{
		try
		{
			// ignore if already deleted
			entry.Refresh();
			if (!entry.Exists)
				return;

			// delete children
			if (entry is DirectoryInfo folder)
			{
				foreach (FileSystemInfo child in folder.GetFileSystemInfos())
					FileHelper.ForceDelete(child);
			}

			// reset permissions & delete
			entry.Attributes = FileAttributes.Normal;
			entry.Delete();

			// wait for deletion to finish
			for (int i = 0; i < 10; i++)
			{
				entry.Refresh();
				if (entry.Exists)
					Thread.Sleep(500);
			}

			// throw exception if deletion didn't happen before timeout
			entry.Refresh();
			if (entry.Exists)
				throw new IOException($"Timed out trying to delete {entry.FullName}");
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed deleting {(entry is DirectoryInfo ? "folder" : "file")} {entry.FullName}", ex);
		}
	}

	/// <summary>Recursively copy a directory or file.</summary>
	/// <param name="source">The file or folder to copy.</param>
	/// <param name="targetFolder">The folder to copy into.</param>
	public static void RecursiveCopy(FileSystemInfo source, DirectoryInfo targetFolder)
	{
		if (!targetFolder.Exists)
			targetFolder.Create();

		switch (source)
		{
			case FileInfo sourceFile:
				sourceFile.CopyTo(Path.Combine(targetFolder.FullName, sourceFile.Name));
				break;

			case DirectoryInfo sourceDir:
				DirectoryInfo targetSubfolder = new(Path.Combine(targetFolder.FullName, sourceDir.Name));
				foreach (FileSystemInfo entry in sourceDir.EnumerateFileSystemInfos())
					FileHelper.RecursiveCopy(entry, targetSubfolder);
				break;

			default:
				throw new NotSupportedException($"Unknown filesystem info type '{source.GetType().FullName}'.");
		}
	}
}