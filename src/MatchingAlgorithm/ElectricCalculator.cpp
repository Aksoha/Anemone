#include "ElectricCalculator.h"

#include <cmath>
#include <numbers>

double ElectricCalculator::CalculateVoltageLimit(const double power, const double maxVoltage, const double maxCurrent,
												 const double nominalResistance, const double turnRatio, const double impedance)
{
	const auto resistanceMinimal = power / (maxCurrent * maxCurrent) / (turnRatio * turnRatio);
	const auto nominalSecondaryResistance = nominalResistance / (turnRatio * turnRatio);

	auto voltage = maxVoltage * sqrt(impedance / nominalSecondaryResistance);

	if(impedance < resistanceMinimal)
		voltage = maxCurrent / sqrt(8 / (std::numbers::pi * std::numbers::pi)) * impedance * turnRatio * turnRatio;

	return voltage > maxVoltage ? maxVoltage : voltage;
}

double ElectricCalculator::CalculatePower(const double resistance, const double impedance, const double voltage, const double turnRatio)
{
	const auto current = CalculateCurrent(impedance, voltage, turnRatio);
	auto power = current * voltage * resistance / impedance;
	power /= 1.11;
	return power;
}

double ElectricCalculator::CalculateCurrent(const double impedance, const double voltage, const double turnRatio)
{
	return voltage / (impedance * turnRatio * turnRatio) / 1.11;
}
