#pragma once
#include "../LlcMatching.h"
#include "../../interop/export.h"

struct LlcMatchingResult;
DLL_EXPORT LlcMatching* LlcMatching_Create(LlcTopology* topology, const LlcMatchingParameter& parameters);
DLL_EXPORT void LlcMatching_Dispose(const LlcMatching* matching);
DLL_EXPORT void LlcMatching_Match(LlcMatching* matching, LlcMatchingResult* results, long length);