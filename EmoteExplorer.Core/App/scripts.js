setTimeout(whenReady, 25);
function whenReady() {
  if (window._spidereye === null) {
    setTimeout(whenReady, 25);
    return;
  }

  // INVOKE SPIDEREYE SHIT
  window._spidereye.invokeApi('Handler.loadSettings', '', (result) => {
    if (!result.value.isDark) {
      document.documentElement.setAttribute('data-bs-theme', 'light');
      themeToggle.value = 'false';
      themeToggle.innerHTML = "<i class='bi bi-sun'></i>";
    }
  });
  search.click();
}

// Page Elements
// Theme Toggle 
var themeToggle = document.getElementById('themeToggle');
// Sort Options
var sort = document.getElementById('sort');
// Image Size Radios
var sizeRadios = document.getElementsByName('size');
// Search
var search = document.getElementById('search');
// Notifications
var notification = document.getElementById('notification')
// Next Page
var nextPage = document.getElementById('nextPage');
// Previous Page
var previousPage = document.getElementById('previousPage');
// Page Settings
var currentPage = 1;
var newSearch = true;


// Back to top Event
window.onscroll = () => {
  let scrollButton = document.getElementById('btn-back-to-top');

  if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
    scrollButton.classList.remove('d-none');
  }
  else {
    scrollButton.classList.add('d-none');
  }

  // Back to top
  scrollButton.onclick = () => {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
  }
};


// Theme Toggle Event
themeToggle.onclick = () => {
  let isDark = themeToggle.value;

  window._spidereye.invokeApi('Handler.themeToggle', {'isDark': isDark},
  (result) => {
    if (result.value.isDark) {
      document.documentElement.setAttribute('data-bs-theme', 'dark');
      themeToggle.value = 'true';
      themeToggle.innerHTML = "<i class='bi bi-moon'></i>";
    }
    else {
      document.documentElement.setAttribute('data-bs-theme', 'light');
      themeToggle.value = 'false';
      themeToggle.innerHTML = "<i class='bi bi-sun'></i>";
    }
  });
};


// Search Box Event
document.getElementById('searchBox').addEventListener('keyup', (event) => {
  if (event.keyCode === 13) {
    event.preventDefault();
    newSearch = true;
    search.click();
  }
});

// Sort Event
sort.onclick = () => {
  if (sort.value == 'count-asc') {
    sort.value = 'count-desc'
    sort.innerHTML = "<i class='bi h3 bi-sort-down'></i>"
    search.click();
  }
  else {
    sort.value = 'count-asc'
    sort.innerHTML = "<i class='bi h3 bi-sort-up'></i>"
    search.click();
  }
};

// Image Size Selection Event
for (let radio of sizeRadios) {
  radio.onclick = () => {
    // We set the radio to checked and the others to unchecked
    radio.checked = true;
    for (let other of sizeRadios) {
      if (other != radio) {
        other.checked = false;
      }
    }
    search.click();
  }
}


// Search Event
search.onclick = () => {
  // Reset Page Number
  if (newSearch) {
    currentPage = 1;
    newSearch = false;
  }

  // Search Parameters
  let searchParam = document.getElementById('searchBox').value;
  let sortOrder = sort.value;
  let query = '?q=' + searchParam + '&sort=' + sortOrder + '&page=' + currentPage;
  
  // Image Size Selection
  let size;
  for (let radio of sizeRadios) {
    if (radio.checked) {
      size = radio;
    }
  }

  // Get Emotes
  window._spidereye.invokeApi('Handler.getEmotes', {'query': query, 'size': size.id},
  (result) => {
    if (result.value.statusOk) {

      // Display Emotes
      document.getElementById('emotesContainer').innerHTML = result.value.html;

      // Emote Actions
      emoteActions();

      // Next Page
      if (result.value.totalPages > currentPage) {
        nextPage.classList.remove('d-none');
        nextPage.onclick = () => {
          currentPage = currentPage + 1;
          search.click();
        }
      }
      else {
        nextPage.classList.add('d-none');
      }

      // Previous Page
      if (currentPage > 1) {
        previousPage.classList.remove('d-none');
        previousPage.onclick = () => {
          currentPage = currentPage - 1;
          search.click();
        }
      }
      else {
        previousPage.classList.add('d-none');
      }

    }
    else {
      // Show Error
      nextPage.classList.add('d-none');
      previousPage.classList.add('d-none');
      document.getElementById('emotesContainer').innerHTML = 
      '<div class="text-center"><h1 class="text-danger-emphasis">' + result.value.html + '</h1></div>';
    }
  });
};


// Send Notification
function sendNotification(response) {
  const rsp = response.split(";")
  var symbolClass = rsp[0] == 'Success' ? ['text-success-emphasis', 'bi-check-circle'] : ['text-danger-emphasis', 'bi-dash-circle'];
  var status = rsp[0];
  var statusMessage = rsp[1];

  document.getElementById('status-symbol').className = 'bi me-2 ' + symbolClass[0] + ' ' + symbolClass[1]
  document.getElementById('status').innerHTML = status
  document.getElementById('statusMessage').innerHTML = statusMessage

  bootstrap.Toast.getOrCreateInstance(notification).show()
}


// Emote Actions
function emoteActions() {
  // Copy Emote Link
  let copyButtons = document.getElementsByClassName('copy');
  for (let button of copyButtons) {
    button.onclick = () => {

      // We get the id for the image element from the button id
      let id = button.id.split('-')[1];
      let link = document.getElementById('emote' + id).getAttribute('src');

      // Copy the link to the clipboard
      window._spidereye.invokeApi('Handler.copyLink', {'html': link},
      (result) => {;
          sendNotification('Success;Link copied');
          notification.onclick = () => {
            bootstrap.Toast.getOrCreateInstance(notification).hide();
          };
      });
    }
  }

  // Save Emote
  let saveButtons = document.getElementsByClassName('save');
  for (let button of saveButtons) {
    button.onclick = () => {

      // We get the id and name for the image element from the button id
      let id = button.id.split('-')[1];
      let link = document.getElementById('emote' + id).getAttribute('src');
      let name = document.getElementById('emote' + id).getAttribute('alt');

      // Save the emote
      window._spidereye.invokeApi('Handler.saveEmote', {'html': link + ',' + name},
      (result) => {
        if (result.value.statusOk) {
          sendNotification('Success;Emote <strong>' + name + '</strong> saved to Pictures/EmoteExplorer');
          notification.onclick = () => {
            window._spidereye.invokeApi('Handler.openEmoteDirectory', '', (result) => {
              bootstrap.Toast.getOrCreateInstance(notification).hide();
            });
          }
        }
        else {
          sendNotification('Error;' + result.value.html);
        }
      });
    }
  }
}