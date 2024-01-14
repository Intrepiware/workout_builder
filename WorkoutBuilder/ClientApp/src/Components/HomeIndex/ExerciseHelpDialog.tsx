import { useEffect } from "react";

interface ExerciseHelpDialogProps {
  youtubeUrl: string;
  onClose: () => void;
}

export function ExerciseHelpDialog({
  youtubeUrl,
  onClose,
}: ExerciseHelpDialogProps) {
  let keyDownListener: any = null;
  useEffect(
    () =>
      (keyDownListener = document.addEventListener("keydown", (event) => {
        if (event.code === "Escape") {
          handleClose();
        }
      })),
    []
  );

  const handleClose = () => {
    document.removeEventListener("keydown", keyDownListener);
    onClose();
  };

  const getEmbedCode = (text: string) => {
    const url = new URL(text);
    let videoId = "";
    let time = 0;
    const params = new URLSearchParams(url.search);
    if (url.hostname.indexOf("youtube.com") >= 0) {
      videoId = params.get("v") || "";
      time = parseFloat(params.get("t")?.replace("s", "") || "");
    } else if (url.hostname === "youtu.be") {
      videoId = url.pathname;
      time = parseFloat(params.get("t") || "");
    }
    return `https://www.youtube.com/embed/${videoId}?autoplay=1${
      time > 0 ? `&start=${time}` : ""
    }`;
  };

  return (
    <div className="modal is-active">
      <div className="modal-background"></div>
      <div className="modal-content">
        <iframe
          id="ytplayer"
          width="480"
          height="270"
          src={getEmbedCode(youtubeUrl)}
          frameBorder="0"
          allowFullScreen
        />
      </div>
      <button
        className="modal-close is-large"
        aria-label="close"
        onClick={() => handleClose()}
      ></button>
    </div>
  );
}
