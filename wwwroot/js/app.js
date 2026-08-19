const API_URL = '/api';
let currentCityName = "Região Metropolitana, PR";

document.addEventListener('DOMContentLoaded', () => {
    initGeolocation();
    initChart();
    loadOccurrences();

    document.getElementById('occurrence-form').addEventListener('submit', createOccurrence);
});

// Captura a geolocalização do navegador
function initGeolocation() {
    if ("geolocation" in navigator) {
        updateCityUI("Buscando localização...");

        const options = {
            enableHighAccuracy: true,
            timeout: 8000,
            maximumAge: 0
        };

        navigator.geolocation.getCurrentPosition(
            async (pos) => {
                const lat = pos.coords.latitude;
                const lon = pos.coords.longitude;
                await fetchCityName(lat, lon);
                fetchWeatherData(lat, lon);
            },
            async (err) => {
                console.warn(`GPS indisponível (${err.code}). Usando Região Metropolitana...`);
                fallbackToMetropolitanRegion();
            },
            options
        );
    } else {
        fallbackToMetropolitanRegion();
    }
}

// Busca a localização e aplica a regra para Região Metropolitana / Itaperuçu
async function fetchCityName(lat, lon) {
    try {
        const res = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lon}&zoom=10`);
        const data = await res.json();
        
        if (data && data.address) {
            const addr = data.address;
            const city = (addr.city || addr.town || addr.village || addr.municipality || addr.county || "").toLowerCase();
            const state = addr.state_code || "PR";

            // Se for Itaperuçu ou Curitiba e arredores da RMC
            if (city.includes("itaperuçu") || city.includes("itaperucu") || city.includes("curitiba") || city.includes("metropolitana")) {
                currentCityName = `Região Metropolitana, ${state}`;
            } else if (city) {
                // Se for outra cidade específica fora da RMC
                const cleanCity = addr.city || addr.town || addr.village || addr.municipality;
                currentCityName = `${cleanCity}, ${state}`;
            } else {
                fallbackToMetropolitanRegion();
                return;
            }
            
            updateCityUI(currentCityName);
            return;
        }
    } catch (e) {
        console.error("Erro na busca de localização:", e);
    }
    
    fallbackToMetropolitanRegion();
}

// Definição padrão caso falhe a localização ou não encontre Itaperuçu diretamente
function fallbackToMetropolitanRegion() {
    currentCityName = "Região Metropolitana, PR";
    updateCityUI(currentCityName);
    fetchWeatherData(-25.22, -49.34);
}

function updateCityUI(cityName) {
    const cityEl = document.getElementById('header-city');
    const inputEl = document.getElementById('location-input');
    
    if (cityEl) cityEl.innerText = cityName;
    if (inputEl) inputEl.value = cityName;
}

// Busca o clima da localização
async function fetchWeatherData(lat, lon) {
    try {
        const res = await fetch(`${API_URL}/weather?lat=${lat}&lon=${lon}`);
        if (!res.ok) throw new Error("Erro na API de clima");
        
        const data = await res.json();

        if (document.getElementById('header-temp-val')) document.getElementById('header-temp-val').innerText = Math.round(data.temperature);
        if (document.getElementById('main-temp')) document.getElementById('main-temp').innerText = Math.round(data.temperature);
        if (document.getElementById('main-feels')) document.getElementById('main-feels').innerText = Math.round(data.apparentTemperature);
        if (document.getElementById('main-humidity')) document.getElementById('main-humidity').innerText = data.humidity;
        if (document.getElementById('stat-max')) document.getElementById('stat-max').innerText = data.apparentTemperature;
        if (document.getElementById('main-risk-title')) document.getElementById('main-risk-title').innerText = data.riskLevel ? data.riskLevel.toUpperCase() : "ATENÇÃO";

        const now = new Date();
        if (document.getElementById('header-time')) {
            document.getElementById('header-time').innerText = now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        }
    } catch (err) {
        console.warn("Não foi possível carregar o clima no momento.", err);
    }
}

// Inicializa o Gráfico
function initChart() {
    const canvas = document.getElementById('weatherChart');
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: ['00:00', '04:00', '08:00', '12:00', '16:00', '20:00'],
            datasets: [
                {
                    label: 'Temperatura (°C)',
                    data: [19, 21, 25, 31, 26, 22],
                    borderColor: '#ff2a6d',
                    backgroundColor: '#ff2a6d',
                    borderWidth: 2,
                    tension: 0.4
                },
                {
                    label: 'Umidade (%)',
                    data: [72, 60, 45, 30, 48, 55],
                    borderColor: '#0252ca',
                    backgroundColor: '#0252ca',
                    borderWidth: 2,
                    tension: 0.4
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { position: 'top' } },
            scales: { y: { beginAtZero: false } }
        }
    });
}

// Carrega as Ocorrências salvas
async function loadOccurrences() {
    try {
        const res = await fetch(`${API_URL}/occurrences`);
        if (!res.ok) throw new Error("Erro ao buscar ocorrências");
        
        const list = await res.json();
        
        const countEl = document.getElementById('stat-count');
        if (countEl) countEl.innerText = list.length;
        
        const container = document.getElementById('occurrences-list');
        if (!container) return;
        
        container.innerHTML = '';

        if (list.length === 0) {
            container.innerHTML = '<div style="font-size:0.8rem; color:#64748b; margin-top:10px;">Nenhuma ocorrência registrada.</div>';
            return;
        }

        list.forEach(item => {
            const id = item.id || item.Id;
            const title = item.title || item.Title;
            const location = item.location || item.Location;
            const riskLevel = item.riskLevel || item.RiskLevel;

            const div = document.createElement('div');
            div.className = 'occurrence-item';
            div.innerHTML = `
                <div>
                    <div style="font-weight:700; font-size:0.85rem;">${title}</div>
                    <div style="font-size:0.75rem; color:#64748b;">${location} • ${riskLevel}</div>
                </div>
                <button class="btn-delete" onclick="deleteOccurrence(${id})">Apagar</button>
            `;
            container.appendChild(div);
        });
    } catch (e) {
        console.error("Erro ao carregar lista de ocorrências:", e);
    }
}

// Registra uma nova ocorrência
async function createOccurrence(e) {
    e.preventDefault();
    const payload = {
        title: document.getElementById('title').value,
        location: document.getElementById('location-input').value,
        riskLevel: document.getElementById('riskLevel').value,
        description: document.getElementById('description').value
    };

    try {
        await fetch(`${API_URL}/occurrences`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        document.getElementById('occurrence-form').reset();
        document.getElementById('location-input').value = currentCityName;
        loadOccurrences();
    } catch (err) {
        alert("Erro ao salvar ocorrência.");
    }
}

// Deleta uma ocorrência pelo ID
async function deleteOccurrence(id) {
    if (confirm("Deseja realmente apagar esta ocorrência?")) {
        try {
            await fetch(`${API_URL}/occurrences/${id}`, { method: 'DELETE' });
            loadOccurrences();
        } catch (err) {
            alert("Erro ao deletar ocorrência.");
        }
    }
}

function openModal() {
    const modal = document.getElementById('info-modal');
    if (modal) modal.style.display = 'flex';
}

function closeModal() {
    const modal = document.getElementById('info-modal');
    if (modal) modal.style.display = 'none';
}