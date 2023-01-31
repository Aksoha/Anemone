#include "gtest/gtest.h"
#include "SweepParameter.h"

namespace MatchingAlgorithmTests
{
	TEST(SweepParameterTest, CastToVector)
	{
		// arrange
		constexpr auto min = 25.6;
		constexpr auto max = 78.3;
		constexpr auto step = 1.13;

		constexpr SweepParameter sweep{ min, max, step };


		// act
		const auto vector = static_cast<std::vector<double>>(sweep);



		// assert
		const auto actualFirstItem = vector.front();
		const auto actualLastItem = vector.back();
		const auto actualSize = static_cast<int>(vector.size());
		const auto expectedSize = static_cast<int>(std::floor((max - min) / step));


		ASSERT_TRUE(std::is_sorted(vector.begin(), vector.end()), L"vector is not sorted in ascending order");
		ASSERT_DOUBLE_EQ(min, actualFirstItem);
		ASSERT_TRUE(actualLastItem <= max);
		ASSERT_TRUE(actualFirstItem - max < step);

		ASSERT_EQ(expectedSize, actualSize);

	}
}