﻿using commonItems;
using commonItems.Localization;
using ImperatorToCK3.CK3.Titles;
using ImperatorToCK3.Mappers.Culture;
using ImperatorToCK3.Mappers.Government;
using ImperatorToCK3.Mappers.Nickname;
using ImperatorToCK3.Mappers.Province;
using ImperatorToCK3.Mappers.Region;
using ImperatorToCK3.Mappers.Religion;
using Xunit;

namespace ImperatorToCK3.UnitTests.CK3.Titles {
	[Collection("Sequential")]
	[CollectionDefinition("Sequential", DisableParallelization = true)]
	public class RulerTermTests {
		[Fact]
		public void ImperatorRulerTermIsCorrectlyConverted() {
			var reader = new BufferedReader(
				"character = 69 " +
				"start_date = 500.2.3 " +
				"government = dictatorship"
			);
			var impRulerTerm = ImperatorToCK3.Imperator.Countries.RulerTerm.Parse(reader);
			var govReader = new BufferedReader("link = {imp=dictatorship ck3=feudal_government }");
			var govMapper = new GovernmentMapper(govReader);
			var imperatorRegionMapper = new ImperatorRegionMapper();
			var ck3RegionMapper = new CK3RegionMapper();
			var ck3RulerTerm = new RulerTerm(impRulerTerm,
				new ImperatorToCK3.CK3.Characters.CharacterCollection(),
				govMapper,
				new LocDB("english"),
				new ReligionMapper(imperatorRegionMapper, ck3RegionMapper),
				new CultureMapper(imperatorRegionMapper, ck3RegionMapper),
				new NicknameMapper("TestFiles/configurables/nickname_map.txt"),
				new ProvinceMapper(),
				new Configuration()
			);
			Assert.Equal("imperator69", ck3RulerTerm.CharacterId);
			Assert.Equal(new Date(500, 2, 3, AUC: true), ck3RulerTerm.StartDate);
			Assert.Equal("feudal_government", ck3RulerTerm.Government);
		}

		[Fact]
		public void PreImperatorTermIsCorrectlyConverted() {
			var countries = new ImperatorToCK3.Imperator.Countries.CountryCollection();
			var countryReader = new BufferedReader("= { tag = SPA capital=420 }");
			var sparta = ImperatorToCK3.Imperator.Countries.Country.Parse(countryReader, 69);
			countries.Add(sparta);

			var preImpTermReader = new BufferedReader("= { name=\"Alexander\"" +
				" birth_date=200.1.1 death_date=300.1.1 throne_date=250.1.1" +
				" nickname=stupid religion=hellenic culture=spartan" +
				" country=SPA }"
			);
			var impRulerTerm = new ImperatorToCK3.Imperator.Countries.RulerTerm(preImpTermReader, countries);

			var govReader = new BufferedReader("link = {imp=dictatorship ck3=feudal_government }");
			var govMapper = new GovernmentMapper(govReader);
			var imperatorRegionMapper = new ImperatorRegionMapper();
			var ck3RegionMapper = new CK3RegionMapper();
			var religionMapper = new ReligionMapper(
				new BufferedReader("link={imp=hellenic ck3=hellenic}"),
				imperatorRegionMapper,
				ck3RegionMapper
			);
			var ck3Characters = new ImperatorToCK3.CK3.Characters.CharacterCollection();
			var ck3RulerTerm = new RulerTerm(impRulerTerm,
				ck3Characters,
				govMapper,
				new LocDB("english"),
				religionMapper,
				new CultureMapper(new BufferedReader("link = { imp=spartan ck3=greek }"), imperatorRegionMapper, ck3RegionMapper),
				new NicknameMapper("TestFiles/configurables/nickname_map.txt"),
				new ProvinceMapper(),
				new Configuration()
			);
			Assert.Equal("imperatorRegnalSPAAlexander504.1.1BC", ck3RulerTerm.CharacterId);
			Assert.Equal(new Date(250, 1, 1, AUC: true), ck3RulerTerm.StartDate);
			var ruler = ck3RulerTerm.PreImperatorRuler;
			Assert.NotNull(ruler);
			Assert.Equal("Alexander", ruler.Name);

			var ck3Character = ck3Characters["imperatorRegnalSPAAlexander504.1.1BC"];
			Assert.Equal(new Date(-555, 1, 1), ck3Character.BirthDate);
			Assert.Equal(new Date(-455, 1, 1), ck3Character.DeathDate);
			Assert.Equal("Alexander", ck3Character.Name);
			Assert.Equal("dull", ck3Character.Nickname);
			Assert.Equal("greek", ck3Character.Culture);
			Assert.Equal("hellenic", ck3Character.Religion);
		}
	}
}
