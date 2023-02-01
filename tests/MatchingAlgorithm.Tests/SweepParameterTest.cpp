#include "gtest/gtest.h"
#include "SweepParameter.h"

namespace MatchingAlgorithmTests
{
	class ConvertToVector_WhenDataIsValid : public testing::TestWithParam<SweepParameter>
	{
	};

	INSTANTIATE_TEST_CASE_P(CastToVector1, ConvertToVector_WhenDataIsValid, testing::Values(
								SweepParameter(20, 30, 1),
								SweepParameter(-15, 13, 0.5),
								SweepParameter(33, 12, 5),
								SweepParameter(12.1, 15.34, 0.18)
							));


	TEST_P(ConvertToVector_WhenDataIsValid, CastToVector)
	{
		// arrange
		const auto parameters = GetParam();

		const auto step = parameters.StepSize;
		const auto min = std::min(parameters.Min, parameters.Max);
		const auto max = std::max(parameters.Min, parameters.Max);

		SweepParameter sweep{min, max, step};


		// act
		std::vector<double> vector;
		try
		{
			vector = static_cast<std::vector<double>>(sweep);
		}
		catch (...)
		{
			GTEST_FAIL();
		}


		// assert
		const auto actualFirstItem = vector.front();
		const auto actualLastItem = vector.back();
		const auto actualSize = static_cast<int>(vector.size());
		const auto expectedSize = static_cast<int>(std::floor((max - min) / step));


		EXPECT_TRUE(std::ranges::is_sorted(vector.begin(), vector.end()));
		EXPECT_DOUBLE_EQ(min, actualFirstItem);
		EXPECT_TRUE(actualLastItem <= max);
		EXPECT_TRUE(actualFirstItem - max < step);

		EXPECT_EQ(expectedSize, actualSize);
	}

	class ConvertToVector_WhenDataIsInvalid : public testing::TestWithParam<SweepParameter>
	{
	};

	INSTANTIATE_TEST_CASE_P(CastToVector2, ConvertToVector_WhenDataIsInvalid, testing::Values(
								SweepParameter(1, 30, 0),
								SweepParameter(0, 0, 15)
							));

	TEST_P(ConvertToVector_WhenDataIsInvalid, CastToVector)
	{
		// arrange
		const auto parameters = GetParam();

		const auto step = parameters.StepSize;
		const auto min = std::min(parameters.Min, parameters.Max);
		const auto max = std::max(parameters.Min, parameters.Max);

		SweepParameter sweep{min, max, step};


		// assert
		ASSERT_THROW(static_cast<std::vector<double>>(sweep);, std::out_of_range);
	}
}
