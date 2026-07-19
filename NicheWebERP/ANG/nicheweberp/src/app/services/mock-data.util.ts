// Small seeded PRNG so mock data is stable across refreshes within a session
// instead of re-randomizing on every reload.
export function mulberry32(seed: number): () => number {
  let a = seed;
  return () => {
    a |= 0;
    a = (a + 0x6d2b79f5) | 0;
    let t = Math.imul(a ^ (a >>> 15), 1 | a);
    t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

export function pick<T>(rng: () => number, items: readonly T[]): T {
  return items[Math.floor(rng() * items.length)];
}

export function daysAgoIso(rng: () => number, maxDays: number): string {
  const days = Math.floor(rng() * maxDays);
  const d = new Date();
  d.setDate(d.getDate() - days);
  return d.toISOString();
}

export const CUSTOMERS = [
  'Chemist Outlet Group', 'MedPlus Pharmacy Co.', 'HealthBridge Retail', 'CarePoint Clinics',
  'Wellness Corner Ltd', 'PharmaCentral', 'Discount Drugstore Chain', 'MediMart Wholesale',
  'GreenCross Pharmacy', 'VitalCare Distributors', 'Naturopath Collective', 'Business Meetings Corp Attire',
  'Muhan Corporate Wear', 'Riverside Medical Supply', 'Sunrise Health Group'
] as const;
