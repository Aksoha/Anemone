#include "LlcMatchingExport.h"

LlcMatching* LlcMatching_Create(LlcTopology* topology, const LlcMatchingParameter& parameters)
{
	auto ptr = std::shared_ptr<LlcTopology>(topology);
	return new LlcMatching(parameters, ptr);
}

void LlcMatching_Dispose(const LlcMatching* matching)
{
	delete matching;
}

void LlcMatching_Match(LlcMatching* matching, LlcMatchingResult* results, long length)
{
	matching->Match();
}
