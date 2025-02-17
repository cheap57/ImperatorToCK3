﻿using commonItems;
using commonItems.Localization;
using ImperatorToCK3.CK3.Characters;
using ImperatorToCK3.CK3.Titles;
using ImperatorToCK3.Imperator.Countries;
using ImperatorToCK3.Mappers.CoA;
using ImperatorToCK3.Mappers.Culture;
using ImperatorToCK3.Mappers.Government;
using ImperatorToCK3.Mappers.Nickname;
using ImperatorToCK3.Mappers.Province;
using ImperatorToCK3.Mappers.Region;
using ImperatorToCK3.Mappers.Religion;
using ImperatorToCK3.Mappers.SuccessionLaw;
using ImperatorToCK3.Mappers.TagTitle;
using ImperatorToCK3.Outputter;
using System.IO;
using Xunit;

namespace ImperatorToCK3.UnitTests.Outputter;

public class CoatOfArmsOutputterTests {
	[Fact]
	public void CoAIsOutputtedForCountryWithFlagSet() {
		var titles = new Title.LandedTitles();

		var countries = new CountryCollection();
		var countryReader = new BufferedReader("tag=ADI flag=testFlag");
		var country = Country.Parse(countryReader, 1);
		countries.Add(country);

		const string outputModName = "outputMod";
		var outputPath = Path.Combine("output", outputModName, "common", "coat_of_arms", "coat_of_arms", "fromImperator.txt");
		SystemUtils.TryCreateFolder(CommonFunctions.GetPath(outputPath));

		var imperatorRegionMapper = new ImperatorRegionMapper();
		var ck3RegionMapper = new CK3RegionMapper();
		titles.ImportImperatorCountries(countries,
			new TagTitleMapper(),
			new LocDB("english"),
			new ProvinceMapper(),
			new CoaMapper("TestFiles/imperatorCoAs.txt"),
			new GovernmentMapper(),
			new SuccessionLawMapper(),
			new DefiniteFormMapper(),
			new ReligionMapper(imperatorRegionMapper, ck3RegionMapper),
			new CultureMapper(imperatorRegionMapper, ck3RegionMapper),
			new NicknameMapper(),
			new CharacterCollection(),
			new Date(400, 1, 1),
			new Configuration()
		);

		CoatOfArmsOutputter.OutputCoas(outputModName, titles);

		using var file = File.OpenRead(outputPath);
		var reader = new StreamReader(file);

		Assert.Equal("d_IMPTOCK3_ADI={", reader.ReadLine());
		Assert.Equal("\tpattern=\"pattern_solid.tga\"", reader.ReadLine());
		Assert.Equal("\tcolor1=red color2=green color3=blue", reader.ReadLine());
		Assert.Equal("}", reader.ReadLine());
	}

	[Fact]
	public void CoAIsNotOutputtedForCountryWithoutFlagSet() {
		var titles = new Title.LandedTitles();

		var countries = new CountryCollection();
		var countryReader = new BufferedReader("tag=BDI");
		var country = Country.Parse(countryReader, 2);
		countries.Add(country);

		const string outputModName = "outputMod";
		var outputPath = Path.Combine("output", outputModName, "common", "coat_of_arms", "coat_of_arms", "fromImperator.txt");
		SystemUtils.TryCreateFolder(CommonFunctions.GetPath(outputPath));

		var imperatorRegionMapper = new ImperatorRegionMapper();
		var ck3RegionMapper = new CK3RegionMapper();
		titles.ImportImperatorCountries(countries,
			new TagTitleMapper(),
			new LocDB("english"),
			new ProvinceMapper(),
			new CoaMapper("TestFiles/imperatorCoAs.txt"),
			new GovernmentMapper(),
			new SuccessionLawMapper(),
			new DefiniteFormMapper(),
			new ReligionMapper(imperatorRegionMapper, ck3RegionMapper),
			new CultureMapper(imperatorRegionMapper, ck3RegionMapper),
			new NicknameMapper(),
			new CharacterCollection(),
			new Date(400, 1, 1),
			new Configuration()
		);

		CoatOfArmsOutputter.OutputCoas(outputModName, titles);

		using var file = File.OpenRead(outputPath);
		var reader = new StreamReader(file);

		Assert.True(reader.EndOfStream);
	}
}