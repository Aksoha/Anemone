#pragma once
#include "export.h"
#include "../HeatingSystem.h"

DLL_EXPORT HeatingSystem* HeatingSystem_Create(const HeatingSystemData* frequency, const HeatingSystemData* temperature,
											   long frequencyLength, long temperatureLength);
DLL_EXPORT void HeatingSystem_Dispose(const HeatingSystem* heatingSystem);
DLL_EXPORT double HeatingSystem_Resistance(const HeatingSystem* heatingSystem, double frequency,
										   double temperature);
DLL_EXPORT double HeatingSystem_Inductance(const HeatingSystem* heatingSystem, double frequency,
										   double temperature);
DLL_EXPORT double HeatingSystem_Impedance(const HeatingSystem* heatingSystem, double frequency,
										  double temperature);
