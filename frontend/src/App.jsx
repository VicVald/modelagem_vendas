import React, { useState, useEffect } from 'react';
import { 
  TrendingUp, 
  ShoppingCart, 
  Package, 
  RefreshCw, 
  AlertCircle, 
  DollarSign, 
  Layers 
} from 'lucide-react';

const API_BASE_URL = 'http://localhost:5276/api/dashboard';

export default function App() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [bestQty, setBestQty] = useState([]);
  const [bestVal, setBestVal] = useState([]);
  const [mostUsedIng, setMostUsedIng] = useState([]);

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [qtyRes, valRes, ingRes] = await Promise.all([
        fetch(`${API_BASE_URL}/melhores-produtos-quantidade`),
        fetch(`${API_BASE_URL}/melhores-produtos-valor`),
        fetch(`${API_BASE_URL}/ingredientes-mais-utilizados`)
      ]);

      if (!qtyRes.ok || !valRes.ok || !ingRes.ok) {
        throw new Error('Erro ao buscar dados do dashboard do servidor.');
      }

      const qtyData = await qtyRes.json();
      const valData = await valRes.json();
      const ingData = await ingRes.json();

      setBestQty(qtyData);
      setBestVal(valData);
      setMostUsedIng(ingData);
    } catch (err) {
      console.error(err);
      setError(err.message || 'Falha na conexão com o servidor do backend.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  // Compute metrics
  const totalSales = bestQty.reduce((acc, curr) => acc + curr.total_vendas, 0);
  const totalRevenue = bestVal.reduce((acc, curr) => acc + curr.valor_total_recebido, 0);
  const avgTicket = totalSales > 0 ? (totalRevenue / totalSales) : 0;
  const topIngredient = mostUsedIng.length > 0 ? mostUsedIng[0] : null;

  // Max values for chart percentage calculation
  const maxQty = bestQty.length > 0 ? Math.max(...bestQty.map(p => p.total_vendas)) : 1;
  const maxRevenue = bestVal.length > 0 ? Math.max(...bestVal.map(p => p.valor_total_recebido)) : 1;
  const maxUsedIng = mostUsedIng.length > 0 ? Math.max(...mostUsedIng.map(i => i.total_utilizacoes)) : 1;

  return (
    <>
      <div className="ambient-glow glow-1"></div>
      <div className="ambient-glow glow-2"></div>

      <div className="dashboard-container">
        {/* Header */}
        <header className="dashboard-header">
          <div className="header-title">
            <h1>Sales Modeling Dashboard</h1>
            <p>Identificação de melhores produtos por quantidade, receita e controle de insumos</p>
          </div>
          <div className="header-actions">
            <button className="btn-refresh" onClick={fetchData} disabled={loading}>
              <RefreshCw className={loading ? 'spin' : ''} size={18} />
              {loading ? 'Atualizando...' : 'Atualizar Dados'}
            </button>
          </div>
        </header>

        {error && (
          <div className="error-message">
            <AlertCircle size={32} style={{ marginBottom: '0.5rem', color: '#ec4899' }} />
            <p>{error}</p>
            <p style={{ fontSize: '0.8rem', marginTop: '0.5rem', color: '#94a3b8' }}>
              Verifique se o backend C# está rodando em http://localhost:5276
            </p>
          </div>
        )}

        {loading && !error && (
          <div className="chart-card" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '400px' }}>
            <div className="loading-wrapper">
              <div className="spinner"></div>
              <p>Carregando dados das SQL Views...</p>
            </div>
          </div>
        )}

        {!loading && !error && (
          <>
            {/* KPI Metrics */}
            <div className="kpis-grid">
              <div className="kpi-card">
                <div className="kpi-info">
                  <h3>Faturamento Total</h3>
                  <p>
                    {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(totalRevenue)}
                  </p>
                </div>
                <div className="kpi-icon icon-purple">
                  <DollarSign size={24} />
                </div>
              </div>

              <div className="kpi-card">
                <div className="kpi-info">
                  <h3>Total de Itens Vendidos</h3>
                  <p>{totalSales}</p>
                </div>
                <div className="kpi-icon icon-cyan">
                  <ShoppingCart size={24} />
                </div>
              </div>

              <div className="kpi-card">
                <div className="kpi-info">
                  <h3>Ticket Médio p/ Item</h3>
                  <p>
                    {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(avgTicket)}
                  </p>
                </div>
                <div className="kpi-icon icon-green">
                  <TrendingUp size={24} />
                </div>
              </div>

              <div className="kpi-card">
                <div className="kpi-info">
                  <h3>Ingrediente Principal</h3>
                  <p style={{ fontSize: '1.25rem', marginTop: '0.4rem', fontWeight: 700 }}>
                    {topIngredient ? `${topIngredient.material_nome} (${topIngredient.total_utilizacoes}x)` : 'Nenhum'}
                  </p>
                </div>
                <div className="kpi-icon icon-pink">
                  <Package size={24} />
                </div>
              </div>
            </div>

            {/* Dashboard Visualizations */}
            <div className="charts-grid">
              
              {/* Chart 1: Faturamento */}
              <div className="chart-card">
                <h2>
                  <DollarSign size={20} style={{ color: 'var(--accent-purple)' }} />
                  Faturamento por Produto (R$)
                </h2>
                <div className="chart-wrapper">
                  <div className="bar-chart-vertical">
                    {bestVal.map((item, index) => {
                      const pct = (item.valor_total_recebido / maxRevenue) * 100;
                      return (
                        <div key={item.produto_id} className="bar-column">
                          <div className="bar-tooltip">
                            <div>Produto #{item.produto_id}</div>
                            <div>Faturamento: {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(item.valor_total_recebido)}</div>
                            <div>Vendas: {item.total_vendas} unid.</div>
                          </div>
                          <div 
                            className="bar-fill" 
                            style={{ 
                              height: `${pct}%`,
                              background: 'linear-gradient(to top, var(--accent-purple), var(--accent-cyan))'
                            }}
                          ></div>
                          <span className="bar-label">Prod #{item.produto_id}</span>
                        </div>
                      );
                    })}
                    {bestVal.length === 0 && <p style={{ color: 'var(--text-secondary)' }}>Sem dados</p>}
                  </div>
                </div>
              </div>

              {/* Chart 2: Quantidade Vendida */}
              <div className="chart-card">
                <h2>
                  <ShoppingCart size={20} style={{ color: 'var(--accent-pink)' }} />
                  Produtos Mais Vendidos (Qtd.)
                </h2>
                <div className="chart-wrapper" style={{ alignItems: 'flex-start', paddingTop: '1rem' }}>
                  <div className="row-chart">
                    {bestQty.slice(0, 7).map((item) => {
                      const pct = (item.total_vendas / maxQty) * 100;
                      return (
                        <div key={item.produto_id} className="row-item">
                          <div className="row-info">
                            <span className="row-label">
                              <span className="badge badge-cyan">ID {item.produto_id}</span>
                              Produto #{item.produto_id}
                            </span>
                            <span className="row-value">{item.total_vendas} vendas</span>
                          </div>
                          <div className="row-bar-container">
                            <div className="row-bar-fill" style={{ width: `${pct}%` }}></div>
                          </div>
                        </div>
                      );
                    })}
                    {bestQty.length === 0 && <p style={{ color: 'var(--text-secondary)', textAlign: 'center' }}>Sem dados</p>}
                  </div>
                </div>
              </div>

              {/* Ingredients Usage */}
              <div className="chart-card full-width">
                <h2>
                  <Layers size={20} style={{ color: 'var(--accent-green)' }} />
                  Utilização de Ingredientes nas Vendas
                </h2>
                <div style={{ overflowX: 'auto' }}>
                  <table className="data-table">
                    <thead>
                      <tr>
                        <th>Ingrediente</th>
                        <th>Unidade</th>
                        <th>Utilizações</th>
                        <th>Frequência Relativa</th>
                      </tr>
                    </thead>
                    <tbody>
                      {mostUsedIng.map((item) => {
                        const pct = (item.total_utilizacoes / maxUsedIng) * 100;
                        return (
                          <tr key={item.material_id}>
                            <td style={{ fontWeight: 600 }}>{item.material_nome}</td>
                            <td><span className="badge">{item.material_medida}</span></td>
                            <td style={{ color: 'var(--accent-cyan)', fontWeight: 700 }}>
                              {item.total_utilizacoes} vezes
                            </td>
                            <td style={{ width: '40%' }}>
                              <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                                <div className="row-bar-container" style={{ flex: 1, height: '8px' }}>
                                  <div 
                                    className="row-bar-fill" 
                                    style={{ 
                                      width: `${pct}%`,
                                      background: 'linear-gradient(to right, var(--accent-green), var(--accent-cyan))'
                                    }}
                                  ></div>
                                </div>
                                <span style={{ fontSize: '0.8rem', fontWeight: 600, color: 'var(--text-secondary)', width: '40px' }}>
                                  {Math.round(pct)}%
                                </span>
                              </div>
                            </td>
                          </tr>
                        );
                      })}
                      {mostUsedIng.length === 0 && (
                        <tr>
                          <td colSpan="4" style={{ textAlign: 'center', color: 'var(--text-secondary)' }}>Sem dados</td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>

            </div>
          </>
        )}
      </div>
    </>
  );
}
