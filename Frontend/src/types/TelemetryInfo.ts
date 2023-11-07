import WellWithContractor from "./WellWithContractor";

export default interface TelemetryInfo extends WellWithContractor {
  wellId: number
  depth: number
  dateTime: string
  telemetryId: number
}