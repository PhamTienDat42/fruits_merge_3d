using System.IO;

using UnityEditor;

namespace Editor
{
	public static class TMP_FontAsset
	{
		[MenuItem("CONTEXT/TMP_FontAsset/Clear Font Atlas", false, 2110)]
		private static void ClearFontAtlas(MenuCommand command)
		{
			var fontAsset = command.context as TMPro.TMP_FontAsset;
			var fontAssetFilePath = AssetDatabase.GetAssetPath(fontAsset);

			var lines = File.ReadAllLines(fontAssetFilePath);
			for (int i = 0; i < lines.Length; ++i)
			{
				if (lines[i].StartsWith("  m_CompleteImageSize:"))
				{
					lines[i] = "  m_CompleteImageSize: 0";
					break;
				}
			}
			for (int i = 0; i < lines.Length; ++i)
			{
				if (lines[i].StartsWith("  image data:"))
				{
					lines[i] = "  image data: 0";
					break;
				}
			}
			for (int i = 0; i < lines.Length; ++i)
			{
				if (lines[i].StartsWith("  _typelessdata:"))
				{
					lines[i] = "  _typelessdata:";
					break;
				}
			}

			File.WriteAllLines(fontAssetFilePath, lines);

			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
		}

		[MenuItem("CONTEXT/TMP_FontAsset/Restore Font Atlas", false, 2111)]
		private static void RestoreFontAtlas(MenuCommand command)
		{
			var fontAsset = command.context as TMPro.TMP_FontAsset;
			var fontAssetFilePath = AssetDatabase.GetAssetPath(fontAsset);

			fontAsset.material.mainTexture = fontAsset.atlasTexture;

			var size = fontAsset.atlasWidth*fontAsset.atlasHeight;
			var lines = File.ReadAllLines(fontAssetFilePath);
			for (int i = 0; i < lines.Length; ++i)
			{
				if (lines[i].StartsWith("  m_CompleteImageSize:"))
				{
					lines[i] = $"  m_CompleteImageSize: {size}";
					break;
				}
			}
			for (int i = 0; i < lines.Length; ++i)
			{
				if (lines[i].StartsWith("  image data:"))
				{
					lines[i] = $"  image data: {size}";
					break;
				}

			}
			for (int i = 0; i < lines.Length; ++i)
			{
				if (lines[i] == "  _typelessdata:")
				{
					lines[i] = $"  _typelessdata: {new string('0', size*2)}";
					break;
				}
			}

			File.WriteAllLines(fontAssetFilePath, lines);

			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
		}

	}
}
