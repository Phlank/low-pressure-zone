using LowPressureZone.Api.Endpoints.Schedules.ClashSlots;
using LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

namespace LowPressureZone.Api.Endpoints.Schedules;

public readonly union SlotResponse(HourlySlotResponse, ClashSlotResponse);