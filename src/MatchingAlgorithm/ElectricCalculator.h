#pragma once

class ElectricCalculator
{
public:
	static double CalculateVoltageLimit(double power, double maxVoltage, double maxCurrent, double nominalResistance,
										double turnRatio, double impedance);
	static double CalculatePower(double resistance, double impedance, double voltage, double turnRatio);
	static double CalculateCurrent(double impedance, double voltage, double turnRatio);
};
