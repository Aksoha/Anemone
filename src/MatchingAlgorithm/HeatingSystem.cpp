#include "HeatingSystem.h"

HeatingSystem::HeatingSystem(const HeatingSystemParameters& resistanceFrequency,
							 const HeatingSystemParameters& inductanceFrequency,
							 const HeatingSystemParameters& resistanceTemperature,
							 const HeatingSystemParameters& inductanceTemperature)
{
	_resistanceFrequency = resistanceFrequency;
	_inductanceFrequency = inductanceFrequency;
	_resistanceTemperature = resistanceTemperature;
	_inductanceTemperature = inductanceTemperature;
}


double HeatingSystem::Resistance(const double frequency, const double temperature) const
{
	return _resistanceFrequency.at(frequency) * _resistanceTemperature.at(temperature);
}

double HeatingSystem::Inductance(const double frequency, const double temperature) const
{
	return _inductanceFrequency.at(frequency) * _inductanceTemperature.at(temperature);
}

std::complex<double> HeatingSystem::Impedance(const double frequency, const double temperature) const
{
	return std::complex{Resistance(frequency, temperature), Inductance(frequency, temperature)};
}