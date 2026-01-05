// Cookie utility for managing player identity

const PLAYER_COOKIE_PREFIX = 'cac_player_';
const CREATOR_COOKIE_PREFIX = 'cac_creator_';
const CHAT_VISIT_COOKIE_PREFIX = 'cac_chat_visit_';

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

/**
 * Get the cookie name for a game creator
 */
function getCreatorCookieName(gameUrl: string): string {
  return `${CREATOR_COOKIE_PREFIX}${gameUrl}`;
}

/**
 * Set a cookie indicating this user created the game
 */
export function setCreatorCookie(gameUrl: string): void {
  const cookieName = getCreatorCookieName(gameUrl);
  // Set cookie to expire in 7 days
  const expires = new Date();
  expires.setDate(expires.getDate() + 7);
  document.cookie = `${cookieName}=true; expires=${expires.toUTCString()}; path=/; SameSite=Strict`;
}

/**
 * Check if the current user is the creator of the game
 */
export function isGameCreator(gameUrl: string): boolean {
  const cookieName = getCreatorCookieName(gameUrl);
  const cookies = document.cookie.split(';');
  
  for (const cookie of cookies) {
    const [name, value] = cookie.trim().split('=');
    if (name === cookieName && value === 'true') {
      return true;
    }
  }
  
  return false;
}

/**
 * Get the cookie name for chat visit timestamp
 */
function getChatVisitCookieName(gameUrl: string): string {
  return `${CHAT_VISIT_COOKIE_PREFIX}${gameUrl}`;
}

/**
 * Set the last chat visit timestamp for a game
 */
export function setChatVisitCookie(gameUrl: string): void {
  const cookieName = getChatVisitCookieName(gameUrl);
  const timestamp = new Date().toISOString();
  // Set cookie to expire in 7 days
  const expires = new Date();
  expires.setDate(expires.getDate() + 7);
  document.cookie = `${cookieName}=${timestamp}; expires=${expires.toUTCString()}; path=/; SameSite=Strict`;
}

/**
 * Get the last chat visit timestamp for a game
 */
export function getChatVisitCookie(gameUrl: string): Date | null {
  const cookieName = getChatVisitCookieName(gameUrl);
  const cookies = document.cookie.split(';');
  
  for (const cookie of cookies) {
    const [name, value] = cookie.trim().split('=');
    if (name === cookieName) {
      const timestamp = new Date(value);
      return isNaN(timestamp.getTime()) ? null : timestamp;
    }
  }
  
  return null;
}
