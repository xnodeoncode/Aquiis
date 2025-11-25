window.sessionTimeoutManager = {
  dotNetRef: null,
  activityEvents: ["mousedown", "mousemove", "keydown", "scroll", "touchstart"],
  isTracking: false,

  initialize: function (dotNetReference) {
    this.dotNetRef = dotNetReference;
    this.startTracking();
    console.log("Session timeout tracking initialized");
  },

  startTracking: function () {
    if (this.isTracking) return;

    const activityHandler = () => {
      this.recordActivity();
    };

    this.activityEvents.forEach((event) => {
      document.addEventListener(event, activityHandler, { passive: true });
    });

    this.isTracking = true;
    console.log("Activity tracking started");
  },

  stopTracking: function () {
    if (!this.isTracking) return;

    const activityHandler = () => {
      this.recordActivity();
    };

    this.activityEvents.forEach((event) => {
      document.removeEventListener(event, activityHandler);
    });

    this.isTracking = false;
    console.log("Activity tracking stopped");
  },

  recordActivity: function () {
    if (this.dotNetRef) {
      try {
        this.dotNetRef.invokeMethodAsync("RecordActivity");
      } catch (error) {
        console.error("Error recording activity:", error);
      }
    }
  },

  dispose: function () {
    this.stopTracking();
    this.dotNetRef = null;
    console.log("Session timeout manager disposed");
  },
};
