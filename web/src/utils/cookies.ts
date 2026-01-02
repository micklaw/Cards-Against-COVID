// Cookie utility for managing player identity

const PLAYER_COOKIE_PREFIX = 'cac_player_';

/**
 * Get the cookie name for a specific game instance
 */
function getPlayerCookieName(gameUrl: string): string {
  return `${PLAYER_COOKIE_PREFIX}${gameUrl}`;
}

/**
 * Set a cookie with the player ID for a specific game
 */
export function setPlayerCookie(gameUrl: string, playerId: number): void {
  const cookieName = getPlayerCookieName(gameUrl);
  // Set cookie to expire in 7 days
  const expires = new Date();
  expires.setDate(expires.getDate() + 7);
  document.cookie = `${cookieName}=${playerId}; expires=${expires.toUTCString()}; path=/; SameSite=Strict`;
}

/**
 * Get the player ID from cookie for a specific game
 */
export function getPlayerCookie(gameUrl: string): number | null {
  const cookieName = getPlayerCookieName(gameUrl);
  const cookies = document.cookie.split(';');
  
  for (const cookie of cookies) {
    const [name, value] = cookie.trim().split('=');
    if (name === cookieName) {
      const playerId = parseInt(value, 10);
      return isNaN(playerId) ? null : playerId;
    }
  }
  
  return null;
}

/**
 * Remove the player cookie for a specific game
 */
export function removePlayerCookie(gameUrl: string): void {
  const cookieName = getPlayerCookieName(gameUrl);
  document.cookie = `${cookieName}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; SameSite=Strict`;
}
