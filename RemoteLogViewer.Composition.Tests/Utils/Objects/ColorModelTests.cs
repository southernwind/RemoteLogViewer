using RemoteLogViewer.Composition.Utils.Objects;

using Shouldly;

namespace RemoteLogViewer.Composition.Tests.Utils.Objects;

/// <summary>
/// <see cref="ColorModel"/> のテストクラスです。
/// </summary>
public class ColorModelTests {
	/// <summary>
	/// <see cref="ColorModel.FromArgb(byte, byte, byte, byte)"/> が正しくプロパティを設定することを確認します。
	/// </summary>
	[Fact]
	public void FromArgb_ShouldSetPropertiesCorrectly() {
		// Arrange
		byte a = 255;
		byte r = 100;
		byte g = 150;
		byte b = 200;

		// Act
		var color = ColorModel.FromArgb(a, r, g, b);

		// Assert
		color.A.ShouldBe(a);
		color.R.ShouldBe(r);
		color.G.ShouldBe(g);
		color.B.ShouldBe(b);
	}

	/// <summary>
	/// <see cref="ColorModel.Equals(ColorModel?)"/> が正しく動作することを確認します。
	/// </summary>
	[Fact]
	public void Equals_ColorModel_ShouldWorkCorrectly() {
		// Arrange
		var color1 = ColorModel.FromArgb(255, 100, 150, 200);
		var color2 = ColorModel.FromArgb(255, 100, 150, 200);
		var color3 = ColorModel.FromArgb(255, 101, 150, 200);

		// Assert
		color1.Equals(null).ShouldBeFalse();
		color1.Equals(color1).ShouldBeTrue();
		color1.Equals(color2).ShouldBeTrue();
		color1.Equals(color3).ShouldBeFalse();
	}

	/// <summary>
	/// <see cref="ColorModel.Equals(object?)"/> が正しく動作することを確認します。
	/// </summary>
	[Fact]
	public void Equals_Object_ShouldWorkCorrectly() {
		// Arrange
		var color1 = ColorModel.FromArgb(255, 100, 150, 200);
		var color2 = ColorModel.FromArgb(255, 100, 150, 200);

		// Assert
		color1.Equals((object?)null).ShouldBeFalse();
		color1.Equals("not a color").ShouldBeFalse();
		color1.Equals((object)color1).ShouldBeTrue();
		color1.Equals((object)color2).ShouldBeTrue();
	}

	/// <summary>
	/// <see cref="ColorModel.GetHashCode"/> が同じ値のオブジェクトに対して同じハッシュコードを返すことを確認します。
	/// </summary>
	[Fact]
	public void GetHashCode_ShouldBeConsistent() {
		// Arrange
		var color1 = ColorModel.FromArgb(255, 100, 150, 200);
		var color2 = ColorModel.FromArgb(255, 100, 150, 200);

		// Assert
		color1.GetHashCode().ShouldBe(color2.GetHashCode());
	}

	/// <summary>
	/// 比較演算子が正しく動作することを確認します。
	/// </summary>
	[Fact]
	public void Operators_ShouldWorkCorrectly() {
		// Arrange
		var color1 = ColorModel.FromArgb(255, 100, 150, 200);
		var color2 = ColorModel.FromArgb(255, 100, 150, 200);
		var color3 = ColorModel.FromArgb(255, 101, 150, 200);
		ColorModel? nullColor = null;

		// Assert
		(color1 == color2).ShouldBeTrue();
		(color1 != color2).ShouldBeFalse();
		(color1 == color3).ShouldBeFalse();
		(color1 != color3).ShouldBeTrue();
		(color1 == nullColor).ShouldBeFalse();
		(nullColor == color1).ShouldBeFalse();
		(nullColor == (ColorModel?)null).ShouldBeTrue();
	}

	/// <summary>
	/// <see cref="ColorModel.ToString"/> が期待される形式の文字列を返すことを確認します。
	/// </summary>
	[Fact]
	public void ToString_ShouldReturnExpectedFormat() {
		// Arrange
		var color = ColorModel.FromArgb(0xFF, 0x12, 0x34, 0x56);

		// Act
		var result = color.ToString();

		// Assert
		result.ShouldBe("#FF123456");
	}
}