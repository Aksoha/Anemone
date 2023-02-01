#include "gtest/gtest.h"
#include "HeatingSystem.h"

using Map = std::unordered_map<double, double>;


TEST(HeatingSystemTest, GetValues_WhenRequestIsValid)
{
	Map rfr;
	Map ifr;
	Map rt;
	Map it;


	rfr.insert({0, 0});
	ifr.insert({0, 0});
	rt.insert({0, 1});
	it.insert({0, 1});

	rfr.insert({50e3, 20e-3});
	ifr.insert({50e3, 2e-6});
	rt.insert({100, 1.15});
	it.insert({100, 1.13});

	HeatingSystem hs(rfr, ifr, rt, it);

	
	const auto impedance = hs.Impedance(0, 0);

	EXPECT_EQ(0, hs.Resistance(0, 0));
	EXPECT_EQ(0, hs.Inductance(0, 0));
	
	
	EXPECT_EQ(20e-3 * 1.15, hs.Resistance(50e3, 100));
	EXPECT_EQ(2e-6 * 1.13, hs.Inductance(50e3, 100));

}

TEST(HeatingSystemTest, GetValues_WhenRequestIsInvalid)
{
	const Map rfr;
	const Map ifr;
	const Map rt;
	const Map it;
	
	const HeatingSystem hs(rfr, ifr, rt, it);
	
	EXPECT_ANY_THROW(hs.Resistance(0, 0));
	EXPECT_ANY_THROW(hs.Inductance(0, 0));
	EXPECT_ANY_THROW(hs.Impedance(0, 0));
}