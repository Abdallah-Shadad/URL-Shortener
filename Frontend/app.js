// ====== CONFIGURATION ======
const BACKEND_URL = 'https://localhost:7174';
const API_BASE_URL = `${BACKEND_URL}/api`;

// ====== STATE ======
let state = {
    token: localStorage.getItem('token') || null,
    username: localStorage.getItem('username') || null,
    isLoginMode: true
};

// ====== DOM ELEMENTS ======
const els = {
    authSection: document.getElementById('auth-section'),
    dashSection: document.getElementById('dashboard-section'),
    userMenu: document.getElementById('user-menu'),
    userGreeting: document.getElementById('user-greeting'),
    authForm: document.getElementById('auth-form'),
    regFields: document.getElementById('register-fields'),
    authTitle: document.getElementById('auth-title'),
    authSubtitle: document.getElementById('auth-subtitle'),
    authBtn: document.getElementById('auth-submit-btn'),
    shortenForm: document.getElementById('shorten-form'),
    longUrlInput: document.getElementById('long-url'),
    urlsList: document.getElementById('urls-list')
};

// ====== INITIALIZATION ======
function init() {
    if (state.token) {
        showDashboard();
    } else {
        showAuth();
    }
}

// ====== API WRAPPER ======
async function apiFetch(endpoint, method = 'GET', body = null) {
    const headers = { 'Content-Type': 'application/json' };
    if (state.token) headers['Authorization'] = `Bearer ${state.token}`;

    const config = { method, headers };
    if (body) config.body = JSON.stringify(body);

    try {
        const res = await fetch(`${API_BASE_URL}${endpoint}`, config);

        if (res.status === 401) {
            handleLogout();
            throw new Error('Session expired. Please login again.');
        }

        const data = res.status === 204 ? null : await res.json();

        if (!res.ok) {
            // Handle ASP.NET Core Validation Errors neatly
            let errorMsg = data?.message || data?.title || 'An error occurred';
            if (data?.errors) {
                errorMsg = Object.values(data.errors)[0][0]; // Get first validation error
            }
            throw new Error(errorMsg);
        }

        return data;
    } catch (error) {
        if (error.message.includes('Failed to fetch')) {
            showToast('Cannot connect to server. Is the API running?', 'error');
        } else {
            showToast(error.message, 'error');
        }
        throw error;
    }
}

// ====== UI LOGIC ======
window.toggleAuthMode = function (mode) {
    state.isLoginMode = mode === 'login';
    const tabs = document.querySelectorAll('.auth-tabs .tab');
    tabs[0].classList.toggle('active', state.isLoginMode);
    tabs[1].classList.toggle('active', !state.isLoginMode);

    els.regFields.classList.toggle('hidden', state.isLoginMode);

    // Toggle requirements
    document.getElementById('reg-name').required = !state.isLoginMode;
    document.getElementById('reg-username').required = !state.isLoginMode;

    els.authTitle.textContent = state.isLoginMode ? 'Sign In' : 'Create Account';
    els.authSubtitle.textContent = state.isLoginMode ? 'Welcome back! Please enter your details.' : 'Join us to start shortening URLs.';
    els.authBtn.innerHTML = `<span>${state.isLoginMode ? 'Sign In' : 'Register'}</span>`;
};

function showAuth() {
    els.authSection.classList.remove('hidden');
    els.dashSection.classList.add('hidden');
    els.userMenu.classList.add('hidden');
}

function showDashboard() {
    els.authSection.classList.add('hidden');
    els.dashSection.classList.remove('hidden');
    els.userMenu.classList.remove('hidden');
    els.userGreeting.innerHTML = `👋 Hello, <strong>${state.username}</strong>`;
    loadUrls();
}

function setBtnLoading(btn, isLoading, originalText) {
    btn.disabled = isLoading;
    btn.innerHTML = isLoading ? '<i class="fa-solid fa-spinner fa-spin"></i>' : `<span>${originalText}</span>`;
}

// ====== AUTHENTICATION ======
els.authForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const email = document.getElementById('auth-email').value;
    const password = document.getElementById('auth-password').value;

    setBtnLoading(els.authBtn, true, '');

    try {
        if (state.isLoginMode) {
            const data = await apiFetch('/auth/login', 'POST', { email, password });
            state.token = data.token;
            state.username = data.username;
            localStorage.setItem('token', data.token);
            localStorage.setItem('username', data.username);
            els.authForm.reset();
            showToast('Successfully logged in!', 'success');
            showDashboard();
        } else {
            const name = document.getElementById('reg-name').value;
            const username = document.getElementById('reg-username').value;

            await apiFetch('/auth/register', 'POST', { name, username, email, password });
            showToast('Account created! Please sign in.', 'success');
            toggleAuthMode('login');
        }
    } catch (error) {
        // Handled in apiFetch
    } finally {
        setBtnLoading(els.authBtn, false, state.isLoginMode ? 'Sign In' : 'Register');
    }
});

document.getElementById('logout-btn').addEventListener('click', handleLogout);

function handleLogout() {
    localStorage.clear();
    state.token = null;
    state.username = null;
    els.urlsList.innerHTML = '';
    showAuth();
}

// ====== URL MANAGEMENT ======
els.shortenForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const btn = document.getElementById('shorten-btn');
    const longUrl = els.longUrlInput.value;

    setBtnLoading(btn, true, '');

    try {
        await apiFetch('/urls', 'POST', { longUrl });
        els.longUrlInput.value = '';
        showToast('URL Shortened Successfully!', 'success');
        loadUrls();
    } catch (error) {
        // Handled in apiFetch
    } finally {
        setBtnLoading(btn, false, 'Shorten');
    }
});

async function loadUrls() {
    els.urlsList.innerHTML = '<div class="empty-state"><i class="fa-solid fa-spinner fa-spin"></i><p>Loading your links...</p></div>';

    try {
        const urls = await apiFetch('/urls');
        renderUrls(urls);
    } catch (error) {
        els.urlsList.innerHTML = '<div class="empty-state"><i class="fa-solid fa-triangle-exclamation"></i><p>Failed to load links.</p></div>';
    }
}

function renderUrls(urls) {
    if (!urls || urls.length === 0) {
        els.urlsList.innerHTML = `
            <div class="empty-state">
                <i class="fa-solid fa-link-slash"></i>
                <h3>No links found</h3>
                <p>Create your first short link using the form above.</p>
            </div>
        `;
        return;
    }

    // Sort by newest first
    urls.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));

    els.urlsList.innerHTML = urls.map(url => `
        <div class="url-card" id="url-${url.id}">
            <div class="url-info">
                <p class="original-url" title="${url.longUrl}">${url.longUrl}</p>
                <a href="${url.shortUrl}" target="_blank" class="short-url">
                    ${url.shortUrl} <i class="fa-solid fa-arrow-up-right-from-square" style="font-size: 0.75rem;"></i>
                </a>
                <p class="url-meta">Created: ${new Date(url.createdAt).toLocaleDateString()}</p>
            </div>
            <div class="url-actions">
                <button class="btn-icon" onclick="copyToClipboard('${url.shortUrl}')" title="Copy">
                    <i class="fa-regular fa-copy"></i>
                </button>
                <button class="btn-icon danger" onclick="deleteUrl(${url.id})" title="Delete">
                    <i class="fa-regular fa-trash-can"></i>
                </button>
            </div>
        </div>
    `).join('');
}

window.copyToClipboard = async function (text) {
    try {
        await navigator.clipboard.writeText(text);
        showToast('Link copied to clipboard!', 'success');
    } catch (err) {
        showToast('Failed to copy text', 'error');
    }
};

window.deleteUrl = async function (id) {
    if (!confirm('Are you sure you want to delete this link?')) return;

    try {
        await apiFetch(`/urls/${id}`, 'DELETE');
        document.getElementById(`url-${id}`).remove();
        showToast('Link deleted successfully', 'success');

        // Refresh empty state if needed
        if (els.urlsList.children.length === 0) loadUrls();
    } catch (error) {
        // Handled in apiFetch
    }
};

// ====== TOAST SYSTEM ======
function showToast(message, type) {
    const toast = document.getElementById('toast');
    const icon = document.getElementById('toast-icon');

    document.getElementById('toast-message').textContent = message;

    toast.className = 'toast'; // Reset
    toast.classList.add(type);

    icon.className = type === 'success'
        ? 'fa-solid fa-circle-check'
        : 'fa-solid fa-circle-exclamation';

    // Force reflow for animation
    void toast.offsetWidth;

    setTimeout(() => toast.classList.add('hidden'), 3000);
}

// Start the app
init();س