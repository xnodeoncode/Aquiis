// File download helper
window.downloadFile = function (fileName, base64Data, mimeType) {
  const linkSource = `data:${mimeType};base64,${base64Data}`;
  const downloadLink = document.createElement("a");
  downloadLink.href = linkSource;
  downloadLink.download = fileName;
  downloadLink.click();
};

// View file in new window using Blob URL
window.viewFile = function (base64Data, mimeType) {
  // Convert base64 to byte array
  const byteCharacters = atob(base64Data);
  const byteNumbers = new Array(byteCharacters.length);
  for (let i = 0; i < byteCharacters.length; i++) {
    byteNumbers[i] = byteCharacters.charCodeAt(i);
  }
  const byteArray = new Uint8Array(byteNumbers);

  // Create blob and object URL
  const blob = new Blob([byteArray], { type: mimeType });
  const blobUrl = URL.createObjectURL(blob);

  // Open in new window
  const newWindow = window.open(blobUrl, "_blank");

  // Clean up the blob URL after a delay to allow the browser to load it
  setTimeout(() => {
    URL.revokeObjectURL(blobUrl);
  }, 100);
};
