#include "HeatingSystem.h"

#include <complex>

HeatingSystem::HeatingSystem(const std::vector<HeatingSystemData>& frequency, const std::vector<HeatingSystemData>&
							 temperature)
{
	for(auto [Key, Resistance, Inductance] : frequency)
	{
		_resistanceFrequency.insert({Key, Resistance});
		_inductanceFrequency.insert({Key, Inductance});
	}

	for(auto [Key, Resistance, Inductance] : temperature)
	{
		_resistanceTemperature.insert({Key, Resistance});
		_inductanceTemperature.insert({Key, Inductance});
	}
}


double HeatingSystem::Resistance(const double frequency, const double temperature) const
{
	return _resistanceFrequency.at(frequency) * _resistanceTemperature.at(temperature);
}

double HeatingSystem::Inductance(const double frequency, const double temperature) const
{
	return _inductanceFrequency.at(frequency) * _inductanceTemperature.at(temperature);
}

double HeatingSystem::Impedance(const double frequency, const double temperature) const
{
	return abs(std::complex{Resistance(frequency, temperature), Inductance(frequency, temperature)});
}