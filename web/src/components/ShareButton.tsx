import React, { useState, useEffect, useRef } from 'react';

interface ShareButtonProps {
  gameUrl: string;
}

const ShareButton: React.FC<ShareButtonProps> = ({ gameUrl }) => {
  const [copied, setCopied] = useState(false);
  const timeoutRef = useRef<number | null>(null);

  // Cleanup timeout on unmount
  useEffect(() => {
    return () => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
    };
  }, []);

  const copyToClipboard = async (url: string) => {
    await navigator.clipboard.writeText(url);
    setCopied(true);
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
    }
    timeoutRef.current = window.setTimeout(() => setCopied(false), 2000);
  };

  const handleShare = async () => {
    const fullUrl = `${window.location.origin}/game/${gameUrl}`;
    
    try {
      // Try using the native Web Share API first (for mobile devices)
      if (navigator.share) {
        await navigator.share({
          title: 'Cards Against COVID',
          text: 'Join my Cards Against COVID game!',
          url: fullUrl,
        });
      } else {
        // Fallback to clipboard
        await copyToClipboard(fullUrl);
      }
    } catch {
      // If share is cancelled or clipboard fails, try clipboard anyway
      try {
        await copyToClipboard(fullUrl);
      } catch (clipboardError) {
        console.error('Failed to share:', clipboardError);
      }
    }
  };

  return (
    <button
      onClick={handleShare}
      className="btn btn-primary btn-sm"
      title="Share game link"
    >
      {copied ? (
        <>
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            strokeWidth={2}
            stroke="currentColor"
            className="w-4 h-4 mr-1"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          Copied!
        </>
      ) : (
        <>
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            strokeWidth={2}
            stroke="currentColor"
            className="w-4 h-4 mr-1"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M7.217 10.907a2.25 2.25 0 100 2.186m0-2.186c.18.324.283.696.283 1.093s-.103.77-.283 1.093m0-2.186l9.566-5.314m-9.566 7.5l9.566 5.314m0 0a2.25 2.25 0 103.935 2.186 2.25 2.25 0 00-3.935-2.186zm0-12.814a2.25 2.25 0 103.933-2.185 2.25 2.25 0 00-3.933 2.185z"
            />
          </svg>
          Share
        </>
      )}
    </button>
  );
};

export default ShareButton;
