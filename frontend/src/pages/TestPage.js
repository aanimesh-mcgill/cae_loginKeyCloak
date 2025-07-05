import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import api from '../services/api';

const TestPage = () => {
    const { isAuthenticated, token, user } = useAuth();
    const [testResults, setTestResults] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const runTests = async () => {
        if (!isAuthenticated || !token) {
            setError('Not authenticated or no token available');
            return;
        }

        setLoading(true);
        setError(null);
        setTestResults(null);

        try {
            // Set token in axios
            api.defaults.headers.common['Authorization'] = `Bearer ${token}`;

            // Test 1: Get all claims
            const claimsResponse = await api.get('/api/Test/claims');
            console.log('Claims response:', claimsResponse.data);

            // Test 2: Test role-based endpoints
            const tests = [];
            
            try {
                const adminResponse = await api.get('/api/Test/admin-only');
                tests.push({ name: 'Admin Only', success: true, data: adminResponse.data });
            } catch (err) {
                tests.push({ name: 'Admin Only', success: false, error: err.response?.status });
            }

            try {
                const editorResponse = await api.get('/api/Test/editor-or-admin');
                tests.push({ name: 'Editor or Admin', success: true, data: editorResponse.data });
            } catch (err) {
                tests.push({ name: 'Editor or Admin', success: false, error: err.response?.status });
            }

            try {
                const anyRoleResponse = await api.get('/api/Test/any-role');
                tests.push({ name: 'Any Role', success: true, data: anyRoleResponse.data });
            } catch (err) {
                tests.push({ name: 'Any Role', success: false, error: err.response?.status });
            }

            setTestResults({
                claims: claimsResponse.data,
                roleTests: tests
            });

        } catch (err) {
            console.error('Test error:', err);
            setError(err.response?.data?.message || err.message);
        } finally {
            setLoading(false);
        }
    };

    if (!isAuthenticated) {
        return (
            <div className="container mt-4">
                <div className="alert alert-warning">
                    Please log in to run JWT role mapping tests.
                </div>
            </div>
        );
    }

    return (
        <div className="container mt-4">
            <h2>JWT Role Mapping Test</h2>
            
            <div className="card mb-4">
                <div className="card-header">
                    <h5>Current User Info</h5>
                </div>
                <div className="card-body">
                    <p><strong>Username:</strong> {user?.preferred_username || 'N/A'}</p>
                    <p><strong>Email:</strong> {user?.email || 'N/A'}</p>
                    <p><strong>Name:</strong> {user?.name || 'N/A'}</p>
                    <p><strong>Roles:</strong> {user?.realm_access?.roles?.join(', ') || 'N/A'}</p>
                    <p><strong>Token:</strong> {token ? `${token.substring(0, 50)}...` : 'N/A'}</p>
                </div>
            </div>

            <div className="mb-4">
                <button 
                    className="btn btn-primary" 
                    onClick={runTests}
                    disabled={loading}
                >
                    {loading ? 'Running Tests...' : 'Run JWT Tests'}
                </button>
            </div>

            {error && (
                <div className="alert alert-danger">
                    <strong>Error:</strong> {error}
                </div>
            )}

            {testResults && (
                <div className="row">
                    <div className="col-md-6">
                        <div className="card">
                            <div className="card-header">
                                <h5>JWT Claims</h5>
                            </div>
                            <div className="card-body">
                                <pre className="bg-light p-3 rounded">
                                    {JSON.stringify(testResults.claims, null, 2)}
                                </pre>
                            </div>
                        </div>
                    </div>
                    
                    <div className="col-md-6">
                        <div className="card">
                            <div className="card-header">
                                <h5>Role-Based Access Tests</h5>
                            </div>
                            <div className="card-body">
                                {testResults.roleTests.map((test, index) => (
                                    <div key={index} className="mb-3">
                                        <h6>{test.name}</h6>
                                        {test.success ? (
                                            <div className="alert alert-success">
                                                <strong>✓ Success:</strong> {test.data.message}
                                            </div>
                                        ) : (
                                            <div className="alert alert-danger">
                                                <strong>✗ Failed:</strong> HTTP {test.error}
                                            </div>
                                        )}
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default TestPage; 