document.addEventListener("DOMContentLoaded", () => {
  /***** Hamburger Menu *****/
  // Get all "navbar-burger" elements
  const $navbarBurgers = Array.prototype.slice.call(
    document.querySelectorAll(".navbar-burger"),
    0
  );

  // Add a click event on each of them
  $navbarBurgers.forEach((el) => {
    el.addEventListener("click", () => {
      // Get the target from the "data-target" attribute
      const target = el.dataset.target;
      const $target = document.getElementById(target);

      // Toggle the "is-active" class on both the "navbar-burger" and the "navbar-menu"
      el.classList.toggle("is-active");
      $target?.classList.toggle("is-active");
    });
  });

  /***** Modal Window *****/
  // Functions to open and close a modal
  function openModal($el: HTMLElement) {
    $el.classList.add("is-active");
  }

  function closeModal($el: HTMLElement) {
    $el.classList.remove("is-active");
  }

  function closeAllModals() {
    (document.querySelectorAll<HTMLElement>(".modal") || []).forEach(
      ($modal) => {
        closeModal($modal);
      }
    );
  }

  // Add a click event on buttons to open a specific modal
  (document.querySelectorAll<HTMLElement>(".js-modal-trigger") || []).forEach(
    ($trigger: HTMLElement) => {
      const modal = $trigger.dataset.target;
      const $target = document.getElementById(modal!) as HTMLElement;

      $trigger.addEventListener("click", (evt) => {
        if (!!$target) {
          openModal($target);
          evt.preventDefault();
        }
      });
    }
  );

  // Add a click event on various child elements to close the parent modal
  (
    document.querySelectorAll<HTMLElement>(
      ".modal-background, .modal-close, .modal-card-head .delete, .modal-card-foot .button"
    ) || []
  ).forEach(($close) => {
    const $target = $close.closest(".modal") as HTMLElement;

    $close.addEventListener("click", () => {
      !!$target && closeModal($target);
    });
  });

  // Add a keyboard event to close all modals
  document.addEventListener("keydown", (event) => {
    if (event.code === "Escape") {
      closeAllModals();
    }
  });
});
